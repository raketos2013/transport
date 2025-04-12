// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




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

var idSelectedTask;
var selectedTask = 0;
var isShowActive = false;
var tableTasks;
var tr;
var isShowActiveAddressee = false;



function MakeRowHover(row, numRow) {
    row.addEventListener("click", function (numRow) {
        //this.style.backgroundColor = "silver";
        let td = this.querySelectorAll('td');
        for (var i = 1; i < tr.length; i++) {
            if (tr[i] == this) {
                //td[1].style.display = 'block';
                tr[i].classList.add('selected-row');
                idSelectedTask = tr[i].querySelectorAll('td')[0].innerText;
                selectedTask = i;
                document.cookie = "selectedTask=" + i;
                document.cookie = "idSelectedTask=" + idSelectedTask;
            }
        }
        for (var i = 1; i < tr.length; i++) {
            if (tr[i] != this) {
                td = tr[i].querySelectorAll('td');
                //td[1].style.display = 'none';
                tr[i].classList.remove('selected-row');
            }
        }
    });
}

function SelectTask() {
    tableTasks = document.getElementById("tableTasks");
    tr = tableTasks.getElementsByTagName("tr");
    for (var i = 0; i < tr.length; i++) {
        MakeRowHover(tr[i], i);
    }
    if (selectedTask != 0) {
        tr[selectedTask].classList.add('selected-row');
    }
}


function ShowActiveTask() {

    isShowActive = !isShowActive;
    if (isShowActive) {
        document.getElementById('titleButton').innerText = "Показать";
    } else {
        document.getElementById('titleButton').innerText = "Скрыть";
    }

    var table, tr, td, i, txtValue, valueCheck;

    table = document.getElementById("tableTasks");
    tr = table.getElementsByTagName("tr");

    // Перебирайте все строки таблицы и скрывайте тех, кто не соответствует поисковому запросу

    for (i = 1; i < tr.length; i++) {
        var qwe = tr[i].getElementsByTagName("td")[7];
        var chl1 = qwe.firstElementChild.firstElementChild.firstElementChild.checked;
        if (isShowActive) {
            if (!chl1) {
                tr[i].style.display = "none";
            }
        } else {
            tr[i].style.display = "";
        }


    }

}

function ShowActiveAddressee() {

    isShowActiveAddressee = !isShowActiveAddressee;
    if (isShowActiveAddressee) {
        document.getElementById('titleButton').innerText = "Показать";
    } else {
        document.getElementById('titleButton').innerText = "Скрыть";
    }

    var table, tr, td, i, txtValue, valueCheck;

    table = document.getElementById("addresseeTable");
    tr = table.getElementsByTagName("tr");

    // Перебирайте все строки таблицы и скрывайте тех, кто не соответствует поисковому запросу

    for (i = 1; i < tr.length; i++) {
        var qwe = tr[i].getElementsByTagName("td")[4];
        var chl1 = qwe.firstElementChild.firstElementChild.firstElementChild.checked;
        if (isShowActiveAddressee) {
            if (!chl1) {
                tr[i].style.display = "none";
            }
        } else {
            tr[i].style.display = "";
        }


    }

}


function ActiveTask() {

    if (isShowActive) {
        document.getElementById('titleButton').innerText = "Показать";
    } else {
        document.getElementById('titleButton').innerText = "Скрыть";
    }

    var table, tr, td, i, txtValue, valueCheck;

    table = document.getElementById("tableTasks");
    tr = table.getElementsByTagName("tr");

    // Перебирайте все строки таблицы и скрывайте тех, кто не соответствует поисковому запросу

    for (i = 1; i < tr.length; i++) {
        var qwe = tr[i].getElementsByTagName("td")[7];
        var chl1 = qwe.firstElementChild.firstElementChild.firstElementChild.checked;
        if (isShowActive) {
            if (!chl1) {
                tr[i].style.display = "none";
            }
        } else {
            tr[i].style.display = "";
        }


    }
}

function ActiveAddressee() {

    if (isShowActiveAddressee) {
        document.getElementById('titleButton').innerText = "Показать";
    } else {
        document.getElementById('titleButton').innerText = "Скрыть";
    }

    var table, tr, td, i, txtValue, valueCheck;

    table = document.getElementById("addresseeTable");
    tr = table.getElementsByTagName("tr");

    // Перебирайте все строки таблицы и скрывайте тех, кто не соответствует поисковому запросу

    for (i = 1; i < tr.length; i++) {
        var qwe = tr[i].getElementsByTagName("td")[4];
        var chl1 = qwe.firstElementChild.firstElementChild.firstElementChild.checked;
        if (isShowActiveAddressee) {
            if (!chl1) {
                tr[i].style.display = "none";
            }
        } else {
            tr[i].style.display = "";
        }


    }
}

function ShowCreateForm() {
    document.getElementById("createForm").style.display = 'block';
    document.getElementById("headr").style.display = 'none';
    document.getElementById("btnCreate").style.display = 'none';
    document.getElementById("btnCancelCreate").style.display = 'block';
}
function HideCreateForm() {
    document.getElementById("createForm").style.display = 'none';
    document.getElementById("headr").style.display = 'block';
    document.getElementById("btnCreate").style.display = 'block';
    document.getElementById("btnCancelCreate").style.display = 'none';
}

function ShowEditForm() {
    document.getElementById("infoOperation").style.display = 'none';
    document.getElementById("editForm").style.display = 'block';
    document.getElementById("btnEdit").style.display = 'none';
    document.getElementById("btnCancelEdit").style.display = 'block';
}
function HideEditForm() {
    document.getElementById("infoOperation").style.display = 'block';
    document.getElementById("editForm").style.display = 'none';
    document.getElementById("btnEdit").style.display = 'block';
    document.getElementById("btnCancelEdit").style.display = 'none';
}



