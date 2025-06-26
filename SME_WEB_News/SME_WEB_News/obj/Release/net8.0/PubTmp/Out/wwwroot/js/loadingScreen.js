/*$(document).ajaxSend(function (event, xhr, options) {

    $('.lsModal').css('display', 'block');

}).ajaxComplete(function (event, xhr, options) {

    $('.lsModal').css('display', 'none');

}).ajaxError(function (event, jqxhr, settings, exception) {

    $('.lsModal').css('display', 'none');

});*/

/*function showModal() {
    document.getElementById("lsModal").style.display = "block";
}

function hideModal() {
    document.getElementById("lsModal").style.display = "none";
}

window.addEventListener("load", function () {
    console.log("Show");
    showModal();
    
});

setTimeout(function () {
    hideModal();
}, 2000);

$(document).ready(function () {
    $('.lsModal').css('display', 'none');
});*/

$(document).ready(function () {
    // Show the loading modal
    $('#lsModal').modal('show');
    console.log("check");
    // Simulate an asynchronous action, e.g., using setTimeout
    setTimeout(function () {
        // After the action is complete, hide the loading modal
        $('#lsModal').modal('hide');
    }, 1500); // Adjust the time according to your needs
});