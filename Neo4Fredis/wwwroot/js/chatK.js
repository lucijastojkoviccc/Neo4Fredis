var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButtonKorisnik").disabled = true;

connection.on("NBP_Chat", function (user, message) {

    connection.invoke("rek").then(function (user) {
        var u = document.getElementById("userKorisnik").innerHTML = user;
    });

    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);

    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {

    connection.invoke("rek").then(function (u) {
        document.getElementById("userKorisnik").innerHTML = u;
    });

    document.getElementById("sendButtonKorisnik").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

var incomingMessageContainer = document.getElementById('incomingMessageContainer');

document.getElementById("salji").addEventListener("click", function (event) {
    var user = document.getElementById("userKorisnik").innerHTML;
    var messageInput = document.getElementById("messageInput");
    var message = messageInput.value;

  //  var message = document.getElementById("messageInput").value;


    //connection.invoke("SendMessageNovo", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});
    connection.invoke("Pub", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("Sub").catch(function (err) {
        return console.error(err.toString());
    });

    // Očisti polje za unos poruke nakon slanja
    messageInput.value = "";

    event.preventDefault();
});



    //dodalaa
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Postavljanje konekcije
connection.start().then(function () {
    console.log("Connected to hub!");
}).catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("salji").addEventListener("click", function (event) {
//    event.preventDefault();
//    var userKorisnik = document.getElementById("userKorisnik").innerText;
//    var messageInput = document.getElementById("messageInput");
//    var message = messageInput.value.trim();

//    if (message !== "") {
//        // Slanje poruke ka hubu
//        connection.invoke("SendMessage", userKorisnik, message).catch(function (err) {
//            return console.error(err.toString());
//        });

//        // Očisti input polje nakon slanja poruke
//        messageInput.value = "";
//    }
//});

//// Prikazivanje pristiglih poruka
//connection.on("ReceiveMessage", function (user, message) {
//    var messagesList = document.getElementById("messagesList");
//    var listItem = document.createElement("li");
//    listItem.innerHTML = `<strong>${user}:</strong> ${message}`;
//    messagesList.appendChild(listItem);

//    // Pomakni skrol na dno kako bi se prikazala poslednja poruka
//    messagesList.scrollTop = messagesList.scrollHeight;
//});

//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//// Postavljanje konekcije
//connection.start().then(function () {
//    console.log("Connected to hub!");
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//document.getElementById("sendButtonKorisnik").addEventListener("click", function (event) {
//    event.preventDefault();
//    var userKorisnik = document.getElementById("userKorisnik").innerText;
//    var messageInput = document.getElementById("messageInput");
//    var message = messageInput.value.trim();

//    if (message !== "") {
//        // Slanje poruke ka hubu
//        connection.invoke("SendMessage", userKorisnik, message).catch(function (err) {
//            return console.error(err.toString());
//        });

//        // Očisti input polje nakon slanja poruke
//        messageInput.value = "";
//    }
//});

//// Prikazivanje pristiglih poruka
//connection.on("ReceiveMessage", function (user, message) {
//    var messagesList = document.getElementById("messagesList");
//    var listItem = document.createElement("li");
//    listItem.innerHTML = `<strong>${user}:</strong> ${message}`;
//    messagesList.appendChild(listItem);

//    // Pomakni skrol na dno kako bi se prikazala poslednja poruka
//    messagesList.scrollTop = messagesList.scrollHeight;
//});

//// Dodajte ovaj deo koda koji će dohvatiti poruke iz Redis-a prilikom učitavanja stranice
//document.addEventListener("DOMContentLoaded", function () {
//    // Dohvatanje poruka sa servera
//    fetch("/ChatController/Receive")
//        .then(response => response.json())
//        .then(data => {
//            if (data) {
//                var messagesList = document.getElementById("messagesList");
//                var listItem = document.createElement("li");
//                listItem.innerHTML = `<strong>${data.korisnik}:</strong> ${data.poruka}`;
//                messagesList.appendChild(listItem);

//                // Pomakni skrol na dno kako bi se prikazala poslednja poruka
//                messagesList.scrollTop = messagesList.scrollHeight;
//            }
//        })
//        .catch(error => console.error("Error fetching messages:", error));
//});

//// Dodajte ovaj deo koda koji će dohvatiti poruke iz Redis-a prilikom učitavanja stranice
//document.addEventListener("DOMContentLoaded", function () {
//    // Dohvatanje poruka sa servera
//    fetch("/Chat/GetMessages")
//        .then(response => response.json())
//        .then(data => {
//            if (data) {
//                var messagesList = document.getElementById("messagesList");

//                // Prikazi sve poruke
//                data.forEach(item => {
//                    var listItem = document.createElement("li");
//                    listItem.innerHTML = `<strong>${item.user}:</strong> ${item.message}`;
//                    messagesList.appendChild(listItem);
//                });

//                // Pomakni skrol na dno kako bi se prikazala poslednja poruka
//                messagesList.scrollTop = messagesList.scrollHeight;
//            }
//        })
//        .catch(error => console.error("Error fetching messages:", error));
//});
