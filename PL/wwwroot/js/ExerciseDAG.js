$(document).ready(function () {
    // Attach a click event handler to the #myButton element
    $("#myButton2").on("click", function () {
        // Call your custom JavaScript function here
        myFunctionDAG();
    });
});

// Define your custom JavaScript function
function myFunctionDAG() {
    // Put your code here that you want to execute after clicking the button
    console.log("Button clicked!");

    // Get the element with class .tf-tree.tf-gap-sm
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm");
    console.log(elements[0]);
    var number = elements[0].innerHTML;

    // Set the hidden input value to the extracted number
    document.getElementById('hiddenNumber').value = number;
}