var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.start().then(function () {

    connection.invoke("ren").then(function (u) {
        document.getElementById("dobrodosao").innerText = u;
    });

    document.getElementById("sendButtonNGO").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

//<script>
//    document.addEventListener('DOMContentLoaded', function () {
//        // Dodajte ovdje skriptu za hvatanje klika na dugme
//        document.querySelectorAll('.zainteresujSeBtn').forEach(function (btn) {
//            btn.addEventListener('click', function () {
//                var mesto = btn.getAttribute('data-mesto');
//                var opisMesta = btn.getAttribute('data-opisMesta');
//                var lokacijaMesta = btn.getAttribute('data-lokacijaMesta');
//                var korisnikEmail = btn.getAttribute('data-korisnikEmail');

//                // Ovdje možete koristiti ove podatke za AJAX poziv ili ih prosljediti funkciji na drugi način
//                zainteresujSe(mesto, opisMesta, lokacijaMesta, korisnikEmail);
//            });
//        });

//    // Funkcija koja šalje podatke na server (AJAX poziv)
//    function zainteresujSe(mesto, opisMesta, lokacijaMesta, korisnikEmail) {
//        // Ovdje dodajte svoj AJAX poziv
//        // Primjer korištenja Fetch API:
//        fetch('/MestoZaIzlaske/PrijaviSe', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json',
//            },
//            body: JSON.stringify({
//                mesto: mesto,
//                opisMesta: opisMesta,
//                lokacijaMesta: lokacijaMesta,
//                korisnikEmail: korisnikEmail
//            }),
//        })
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error('Network response was not ok');
//                }
//                // Ovdje možete dodati logiku ako je poziv bio uspješan
//            })
//            .catch(error => {
//                console.error('There has been a problem with your fetch operation:', error);
//            });
//        }
//    });
//</script>

