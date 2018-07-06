function send_prn() {
    let photo = document.getElementById("image-file").files[0] // get file from input

    let formData = new FormData();
    formData.append("photo", photo);
    // formData.append("user", JSON.stringify(user));   // you can add also some json data to formData like e.g. user = {name:'john', age:34}

    let xhr = new XMLHttpRequest();
    xhr.open("POST", 'print');
    xhr.send(formData);
}


function init() {
    loading = true;
    ws = new WebSocket('ws://127.0.0.1:3044/chat');

    //Connection open event handler
    ws.onopen = function (evt) {
        ws.send(JSON.stringify({ action: 'connect', name: name, role: role }));
    }

    ws.onerror = function (msg) {
        alert('socket error:' + msg.toString());
    }

    //if their socket closes unexpectedly, re-establish the connection
    ws.onclose = function () {
        init();
    }

    //Event Handler to receive messages from server
    ws.onmessage = function (message) {
        console.log('Client - received socket message: ' + message.data.toString());
        document.getElementById('loading').style.display = 'none';

        if (message.data) {

            obj = message.data;

            if (obj.userList) {

                //remove the current users in the list
                userListElement = document.getElementById('userList');

                while (userListElement.hasChildNodes()) {
                    userListElement.removeChild(userListElement.lastChild);
                }

                //add on the new users to the list
                for (var i = 0; i < obj.userList.length; i++) {

                    var span = document.createElement('span');
                    span.className = 'user';
                    span.style.display = 'block';
                    span.innerHTML = obj.userList[i].name;
                    userListElement.appendChild(span);
                }
            }
        }

        if (message.data === '__ping__') {
            ws.send(JSON.stringify({ keepAlive: name }));
        }

        return false;
    }
}
var ws;
function run_web_socket() {
   
    ws = new WebSocket('ws://127.0.0.1:3044/chat');
        

    // Log messages from the server
    ws.onmessage = function (e) {
        console.log(e);
    };
    ws.onerror = function (e) {
        console.log("error:");
        console.log(e);
    };
    ws.onopen = function () {
        ws.send('heartbeat');
        console.log("Connection opened...");
    };

    // второй - когда соединено закроется
    ws.onclose = function () { console.log("Connection closed...") };
  //  CONNECTION.send('Hellow World');
}

console.log("js loaded");
run_web_socket();