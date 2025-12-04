function initVue(modelData) {
    const app = Vue.createApp({
        data() {
            return {
                employeeInfo: modelData,
                employeeData: modelData.Employees,
                filter: "",
                filterTeam: "None",
                filterEventType: "None",
                firstDate: new Date(modelData.BeginDate),
                accumulateDays: this.getEmptyDays(modelData.Employees, modelData.IdEmployee),
                selectedDate: null,
                selectedDatetime: null,
                newRequestOption: "HOLIDAY",
                remoteDaysSelection: []
            }
        },
        computed: {
            // Crea in modo dinamico una lista di dipenndeti filtrati per nome, cognome, evento e team
            filteredList() {
                // Elimina gli spazi vuoti
                const search = this.filter.toLowerCase().trim();

                // Ritorna direttamente se nessun filtro è stato applicato
                if (!search && this.filterTeam === "None" && this.filterEventType === "None") {
                    this.accumulateDays = this.getEmptyDays(this.employeeData, this.employeeInfo.IdEmployee)
                    return this.employeeData;
                }

                // Filtra il dipendente in base al team e/o in baes al cognome e/o nome
                // Questo è un filtro su una proprietà primaria
                let firstFilteredList = this.employeeData.filter(x => {
                    let filterByName = true;
                    let filterByTeam = true;

                    // Filtra su nome e/o cognome
                    if (search) {
                        const surname = x.Surname.toLowerCase();
                        const name = x.Name.toLowerCase();

                        filterByName = (surname.includes(search) || name.includes(search) || `${surname} ${name}`.includes(search))
                    }

                    // Filtra per team
                    if (this.filterTeam !== "None") {
                        filterByTeam = x.Team === this.filterTeam
                    }

                    // ritorna la concatenazione della condizione: se è tutto vero allora il dipendente viene tenuto altrimenti viene filtrato via
                    return filterByName && filterByTeam;
                })


                // Filtra per evento su ogni singolo dipendente
                // In questo caso si tratta invece di filtrare, su ogni dipendente, la sua lista di eventi quindi fare un filtro su una proprietà secondaria
                if (this.filterEventType === "None") {
                    this.accumulateDays = this.getEmptyDays(firstFilteredList, this.employeeInfo.IdEmployee)
                    return firstFilteredList;
                }

                // Se invece c'è il filtro per evento
                let filteredListFinal = firstFilteredList
                    .map(x => ({
                        // ricopia tutti i campi tranne quello degli eventi
                        ...x,

                        // Fai il filtro sugli eventi
                        Events: x.Events.flatMap(eventObject => {
                            // Per ogni dipendente, c'è una lista di giorni (DAY):
                            // Ogni giorno (DAY) comprende un numero di giorni per cui continua il determinato giorno.
                            // Ogni giorno (DAY) può comprendere una lista di null ooppure una lista di 1 o più eventi (DAYEVENT)

                            // Per ogni DAY si guardano i singoli DAYEVENT
                            // Filtra solo gli eventi che sono uguali al nome dell'evento passato come filtro
                            // è possibile che la lista degli eventi per il singolo giorno sia null 
                            // quindi dinamicamanete crea un array vuoto in questi casi per evitare eccezioni
                            const filteredListEvents = (eventObject.ListEvents ?? []).filter(
                                singleEvent =>
                                    singleEvent.Type.startsWith(this.filterEventType.charAt(0))
                            );

                            // Se un certo DAY comprende più giorni e il DAYEVENT contenuto al suo interno non è quello uguale al fitro,
                            // Si avra un DAY comprendente più giorni e senza eventi
                            // Bisogna separare i giorni quando questi coprono più giorni
                            // Es. c'era un holiday che copriva 4 giorni ma è stato filtrato per leave:
                            // in questo caso si separano il singolo evento da 4 giorni in 4 singoli giorni

                            // Se il giorno contiene 1 singolo giorno oppure la lista degli eventi per quel singolo giorno contiene degli eventi
                            // allora ritorna semplicemnte quel giorno così com'è stato filtrato
                            // Se comprende 1 singolo giorno, non c'è bisogno di spacchettare nulla
                            // Se lista di DAYEVENTS contiene qualcosa, bisogna restituire il singolo DAY così com'è perchè contiene ancora eventi da visualizzare
                            if (eventObject.Days === 1 || filteredListEvents.length > 0) {
                                return {
                                    ...eventObject,
                                    ListEvents: filteredListEvents
                                };
                            }

                            // Se invece il DAY comprende più giorni e non ci sono DAYEVENT perchè nessuno corrisponde al filtro,
                            // Si spacchetta questo DAY da più giorni in singolo DAY da 1 singolo giorno
                            // Caso speciale: Days > 1 e ListEvents vuoto allora spacchetta
                            const startDate = new Date(eventObject.Date);

                            // In pratica si va a lavorare sul singolo DAY andando a ricreare:
                            // La nuova data che rappresenta 1 singolo DAY
                            // Il numero di giorni che comprende questo DAY che comprende ora di default 1 singolo giorno
                            // Una lista di DAYEVENT vuota
                            return Array.from({ length: eventObject.Days }, (_, i) => ({
                                ...eventObject,
                                Date: new Date(
                                    startDate.getTime() + i * 24 * 60 * 60 * 1000
                                ).toISOString(),
                                Days: 1,
                                ListEvents: []
                            }));
                        })
                    }))
                    // Mantieni solo gli employee che hanno almeno un evento con ListEvents non vuoto
                    .filter(x =>
                        x.Events.some(eventObject => (eventObject.ListEvents ?? []).length > 0)
                );

                this.accumulateDays = this.getEmptyDays(filteredListFinal, this.employeeInfo.IdEmployee)

                return filteredListFinal;
            },
            // Crea in modo dinamico l'end date del leave in base al begin date
            maxDateLeave() {
                let splitDate = this.selectedDatetime.split("-");
                let splitDay = splitDate[2].split("T");

                return `${splitDate[0]}-${splitDate[1]}-${splitDay[0]}T18:00`;
            },
        },
        methods: {
            // Crea in modo dinamico il background degli eventi (usato solo per holiday in quanto è l'unico che colora la cella completamente):
            // Rosso per holiday 
            // A strisce per le richieste di holiday non ancora accettate
            getBackgroundColor(list) {
                if (list.length > 0) {
                        if (list[0].Type.startsWith("H")) {

                            if (list[0].Status.includes("PENDING")) {
                                return 'pendingRequest'
                            }
                        return 'bg-danger'
                    }
                }

                return 'bg-white'
            },
            // Crea in modo dinamico il colore del testo e il background 
            getTextColor(event) {
                if (event != null) {
                    // Definisci il colore di testo dell'holiday
                    if (event.Type.startsWith("H")) {
                        if (event.Status.includes("PENDING")) {
                            return 'text-dark'
                        }

                        return 'text-white'

                     // In caso di leave, viene creato un badge con background rosso 
                    } else if (event.Type.startsWith("L")) {
                        if (event.Status.includes("PENDING")) {
                            return `badge text-dark pendingRequest fs-6`
                        }
                        return 'badge text-bg-danger fs-6'
                    }
                }

                return 'text-black'
            },
            // Utility method per formattare le date in formato dd-MM-yyyy
            formatDate(dateStr) {
                return new Date(dateStr).toLocaleDateString("it-IT", {
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric"
                });
            },
            // Metodo usato per assegnare alle celle del dipendente loggato le date come id
            getEmptyDays(employees, id) {
                // Trova la lista degli eventi per il dipendente loggato
                let eventsPerGivenId = employees.filter(x => x.Id === id)

                // Se la ricerca è vuota quindi il dipendente non è presente nella lista dei dipendenti passata, allora non fare null
                if (eventsPerGivenId.length === 0) {
                    eventsPerGivenId = [];
                } else {
                    // Se il dipendente è presnete in lista allora preleva la sua lista degli eventi dai risultati della ricerca
                    eventsPerGivenId = eventsPerGivenId[0].Events;
                }

                // Estrea dalla lista degli eventi, solo le date e poi le formatta
                let dateArray = eventsPerGivenId.map(x => x.Date).map(this.formatDate);

                return dateArray;
            },
            // Metodo per aprire l'offcanvas con le date corrette
            openOffCanvas(event) {
                if (event.target.classList.contains("holiday")) {
                    return;
                }

                let splitDate = event.target.id.split("/");
                this.selectedDate = `${splitDate[2]}-${splitDate[1]}-${splitDate[0]}`;
                this.selectedDatetime = `${splitDate[2]}-${splitDate[1]}-${splitDate[0]}T09:00`;

                const offcanvasEl = this.$refs.offcanvas;
                const offcanvas = new bootstrap.Offcanvas(offcanvasEl);
                offcanvas.show();
            },
            // Metodo usato per resettare tutti i filtri
            resetFilters() {
                this.filterTeam = "None";
                this.filterEventType = "None";
                this.accumulateDays = this.getEmptyDays(modelData.Employees, this.employeeInfo.IdEmployee)
            },
            // Metodo usato per selezionare
            selectRemoteDay(event) {
                const day = event.target.id.split("_")[1];
                const index = this.remoteDaysSelection.findIndex(x => x === day);

                if (index !== -1) {
                    this.remoteDaysSelection.splice(index, 1);
                } else {
                    this.remoteDaysSelection.push(day)
                }

                event.target.classList.toggle("selected-remote-day");

                console.log(this.remoteDaysSelection);
            }
        },
        // Metodo di bootstrap per caricare i tooltip
        mounted() {
            const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
            const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
        }

    })

    app.mount('#app')
}

initVue(serverModel);

