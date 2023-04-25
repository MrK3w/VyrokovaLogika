var contradiction = false;

$(document).ready(function () {
    var currentUrl = window.location.href;

    // Check if the URL contains the query parameter "?handler=Exercise"
    if (currentUrl.indexOf("?handler=Exercise") !== -1) {
        // Attach a click event handler to the .tf-nc span element
        $(".tf-nc").on("click", function () {
            // Get the current span element
            console.log("click");
            var spanElement = $(this);

            // Get the current content of the span element
            var currentContent = spanElement.text();
            //replace 1 to 0 and otherwise
            if (!contradiction) {
                var newContent = currentContent.replace("0", "#").replace("1", "0").replace("#", "1");
                spanElement.text(newContent);
            }
            //if we want to add x for contradiction
            else { 
                //if it is literal add or remove x
                if ((/^[A-Za-z]/.test(currentContent))) {
                    var hasX = currentContent.indexOf("x") !== -1;

                    var newContent = hasX ? currentContent.replace(" x", "") : currentContent + " x";
                    spanElement.text(newContent);
                }
            }
            // Modify the content of the span element
           
        });

        // Attach a click event handler to the #xButton element
        $("#xButton").on("click", function () {
            // Get the current value of the hiddenNumber input element
            contradiction = !contradiction;
            // Check if the hiddenNumber contains a number
        });
    }
});

//case of interactive tree
$(document).ready(function () {
    var currentUrl = window.location.href;

    if (currentUrl.indexOf("?handler=InteractiveTree") !== -1) {
        $(".tf-nc").on("click", function () {
            // Get the current span element

            var spanElement = $(this);

            // Get the current content of the span element
            var currentContent = spanElement.text();

            // Define an object with key-value pairs for replacements
            var replacements = {
                "∨": "∧",
                "∧": "¬",
                "¬": "⇒",
                "⇒": "≡",
                "≡": "¬¬",
                "¬¬": "∨",
            };
            
            // Replace the current content with new content based on replacements
            var newContent = "";
            var char = currentContent;
            //if it is not literal we can change operator of node by click
            if (replacements.hasOwnProperty(char)) {
                newContent = replacements[char];
            } else if (!/[a-zA-Z]/.test(char)) {
                newContent = "∨";
            }
            else {
                newContent = currentContent;
            }
            spanElement.text(newContent);
        });
    }
});

$(document).ready(function () {
    // Attach a click event handler to the #myButton element
    $("#myButton").on("click", function () {
        // Call your custom JavaScript function here
        myFunction();
    });
});

$(document).ready(function () {
    // Attach a click event handler to the #myButton element
    $("#myButton3").on("click", function () {
        // Call your custom JavaScript function here
        myFunction3();
    });
});
//prepare button for dag
$(document).ready(function () {
    $("#myButton2").on("click", function () {
        myFunctionDAG();
    });
});

//functions to get tree from page
function myFunction() {


    // Get the element with class .tf-tree.tf-gap-sm
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm");
    console.log(elements[0]);
    var number = elements[0].innerHTML;

    // Set the hidden input value to the extracted number
    document.getElementById('hiddenNumber').value = number;
}

function myFunction3() {
    // Get the element with class .tf-tree.tf-gap-sm
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm");
    console.log(elements[0]);
    var number = elements[0].innerHTML;

    // Set the hidden input value to the extracted number
    document.getElementById('hiddenTree').value = number;
}