var socket;
window.onload = function () {
    socket = new WebSocket("ws://192.168.180.187:9001");
    socket.onmessage = function (rpt) {
        var mensaje = rpta.data;
        var cadena = `<div class="text-registrado" > <span class="sp-regis" > ${mensaje} </span></div >`;
        chat.insertAjacentHTML("beforeend", cadena);
    }
}
function Enviar() {
    var chat = document.getElementById("chat_s");
    var mensaje = get("mensaje");

    socket.send(mensaje);
    set("mensaje", "")
}