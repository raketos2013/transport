// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/*function ViewAddForm(par) {
    if (par == 1) {
        document.getElementById("addForm").style.display = "none"
        
    } else if (par == 2) {
        document.getElementById("addForm").style.display = "flex"
    }
};

function ViewEditForm(par) {
    if (par == 1) {
        document.getElementById("EditForm").style.display = "none"

    } else if (par == 2) {
        document.getElementById("addForm").style.display = "flex"
    }
};


function ViewAddOper(par) {
    if (par == 1) {
        document.getElementById("addOper").style.display = "none"

    } else if (par == 2) {
        document.getElementById("addOper").style.display = "flex"
    }
};


function ViewAddGroup(par) {
    if (par == 1) {
        document.getElementById("addGroup").style.display = "none"

    } else if (par == 2) {
        document.getElementById("addGroup").style.display = "flex"
    }
};*/


window.addEventListener('unload', function (event) {
    document.cookie = "ddddd" + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
});





function myFunction(select) {
    // Объявить переменные
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable");
    tr = table.getElementsByTagName("tr");

    // Перебирайте все строки таблицы и скрывайте тех, кто не соответствует поисковому запросу
    var col = select.options[select.selectedIndex].value
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[col];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}