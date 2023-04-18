$(document).ready(function () {
    // Attach a click event handler to the .tf-nc span element
    $(".tf-nc").on("click", function () {
        // Get the current span element
        var spanElement = $(this);

        // Get the current content of the span element
        var currentContent = spanElement.text();
        var newContent = currentContent.replace("0", "#").replace("1", "0").replace("#", "1");

        // Modify the content of the span element
        spanElement.text(newContent);
    });
});

$(document).ready(function () {
    // Attach a click event handler to the #myButton element
    $("#myButton").on("click", function () {
        // Call your custom JavaScript function here
        myFunction();
    });
});

// Define your custom JavaScript function
function myFunction() {


    // Get the element with class .tf-tree.tf-gap-sm
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm");
    console.log(elements[0]);
    var number = elements[0].innerHTML;

    // Set the hidden input value to the extracted number
    document.getElementById('hiddenNumber').value = number;
}