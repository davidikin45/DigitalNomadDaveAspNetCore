var myEventMethod = window.addEventListener ? "addEventListener" : "attachEvent";
var myEventListener = window[myEventMethod];
var myEventMessage = myEventMethod == "attachEvent" ? "onmessage" : "message";
myEventListener(myEventMessage, function (e) {
if (e.data === parseInt(e.data))
{
    document.getElementById('dynamicFormIFrame').height = e.data + "px";
}
}, false);
window.onload = function () {
    var link = "{absoluteFormUrl}/" + window.location.search;
    var iframe = document.createElement('iframe');
    iframe.frameBorder = 0;
    iframe.style.width = '100%';
    iframe.height = '600px';
    iframe.id = 'dynamicFormIFrame';
    iframe.setAttribute("src", link);
    iframe.setAttribute("scrolling", "no");
    document.getElementById("dynamicForm").appendChild(iframe);
}