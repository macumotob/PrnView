function send_prn() {
    let photo = document.getElementById("image-file").files[0] // get file from input

    let formData = new FormData();
    formData.append("photo", photo);
    // formData.append("user", JSON.stringify(user));   // you can add also some json data to formData like e.g. user = {name:'john', age:34}

    let xhr = new XMLHttpRequest();
    xhr.open("POST", 'print');
    xhr.send(formData);
}
alert("ok");