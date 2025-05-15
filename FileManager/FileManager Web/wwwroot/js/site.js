// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




window.addEventListener('unload', function (event) {
    document.cookie = "ddddd" + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
});


/*function myFunction(select) {
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


*/


/*function MakeRowHover(row, numRow) {
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

*/

/*function ActiveAddressee() {

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
}*/


var idSelectedTask;
var selectedTask = 0;
var isShowActive = false;
var tableTasks;
var tr;
var isShowActiveAddressee = false;
function ShowActiveTask() {

    isShowActive = !isShowActive;
    if (isShowActive) {
        document.getElementById('titleButton').innerText = "Показать ";
    } else {
        document.getElementById('titleButton').innerText = "Скрыть ";
    }

    var table, tr, td, i, txtValue, valueCheck;

    table = document.getElementById("tableTasks");
    tr = table.getElementsByTagName("tr");

    // Перебирайте все строки таблицы и скрывайте тех, кто не соответствует поисковому запросу

    for (i = 1; i < tr.length; i++) {
        var qwe = tr[i].getElementsByTagName("td")[8];
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
        var qwe = tr[i].getElementsByTagName("td")[8];
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




/*Сворачивает меню*/
function HideMenu() {
    var main_part = document.querySelector(".main-part");
    var menu_open = document.getElementById("menu-open");
    var menu_close = document.getElementById("menu-close");
    var menu_item_text = document.querySelectorAll(".menu-item span");
    var menu_item_svg = document.querySelectorAll(".menu-item svg");
    var left_menu = document.getElementById("left-menu");
    var menu_logo = document.getElementById("menu-logo");
    var header_logo = document.getElementById("header-logo");
    var sub_menu_tasks = document.querySelector(".sub-menu#sub-menu-tasks");
    var sub_menu_groups = document.querySelector(".sub-menu#sub-menu-groups");
    main_part.setAttribute('style', 'width: calc(100vw - 54px);');
    menu_open.style.display = "block";
    menu_close.style.display = "none";
    for (i = 0; i < menu_item_text.length; i++) {
        menu_item_text[i].setAttribute('style', 'display:none;')
    }
    left_menu.setAttribute('style', 'width:54px;');
    menu_logo.style.display = "none";
    for (i = 0; i < menu_item_svg.length; i++) {
        menu_item_svg[i].setAttribute('style', 'margin-right:0px;')
    }
    menu_open.setAttribute('style', 'margin-right:0px;');
    sub_menu_tasks.setAttribute('style', 'left: 54px;');
    sub_menu_groups.setAttribute('style', 'left: 54px;');
    header_logo.setAttribute('style', 'visibility: visible;');
}

/*Раскрывает меню*/
function OpenMenu() {
    var main_part = document.querySelector(".main-part");
    var menu_open = document.getElementById("menu-open");
    var menu_close = document.getElementById("menu-close");
    var menu_item_text = document.querySelectorAll(".menu-item span");
    var menu_item_svg = document.querySelectorAll(".menu-item svg");
    var left_menu = document.getElementById("left-menu");
    var menu_logo = document.getElementById("menu-logo");
    var header_logo = document.getElementById("header-logo");
    var sub_menu_tasks = document.querySelector(".sub-menu#sub-menu-tasks");
    var sub_menu_groups = document.querySelector(".sub-menu#sub-menu-groups");
    main_part.setAttribute('style', 'width: calc(100vw - 235px);');
    menu_open.style.display = "none";
    menu_close.style.display = "block";
    for (i = 0; i < menu_item_text.length; i++) {
        menu_item_text[i].setAttribute('style', 'display:block;')
    }
    left_menu.setAttribute('style', 'width:235px;');
    menu_logo.style.display = "block";

    for (i = 0; i < menu_item_svg.length; i++) {
        menu_item_svg[i].setAttribute('style', 'margin-right: 12px;')
    }
    sub_menu_tasks.setAttribute('style', 'left: 235px;');
    sub_menu_groups.setAttribute('style', 'left: 235px;');
    header_logo.setAttribute('style', 'visibility: hidden;');
}

/*Меню второго уровня*/
function ShowHideSubMenu(element_id) {
    if (document.getElementById(element_id)) {
        var obj = document.getElementById(element_id);
        if (obj.style.display != "block") {
            obj.style.display = "block";
        }
        else obj.style.display = "none";
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const block2 = document.getElementById('sub-menu-groups');
    window.addEventListener('click', e => {
        const target = e.target
        if (!target.closest('#sub-menu-groups') && !target.closest('.menu-item.groups')) {
            block2.style.display = "none";
        }
    })
})

document.addEventListener('DOMContentLoaded', () => {
    const block = document.getElementById('sub-menu-tasks');
    window.addEventListener('click', e => {
        const target = e.target
        if (!target.closest('#sub-menu-tasks') && !target.closest('.menu-item.tasks')) {
            block.style.display = "none";
        }
    })
})

document.addEventListener('DOMContentLoaded', () => {
    const block = document.getElementById('filter');
    window.addEventListener('click', e => {
        const target = e.target
        if (!target.closest('#filter') && !target.closest('.btn-full.btn-filter')) {
            block.style.display = "none";
        }
    })
})

function ShowModal(element_id) {
    document.getElementById(element_id).style.display = "flex";
}

function CloseModal(element_id) {
    document.getElementById(element_id).style.display = "none";
}


function TaskDetails(taskId) {
    $.ajax({
        method: 'GET',
        url: '/Task/TaskDetails',
        data: { "taskId": taskId },
        dataType: 'html',
        success: function (result) {
            $('#task-details-content').empty();
            $('#task-details-content').append(result);
            ShowModal('modal-task-info');
        }
    });
}


function EditTask(taskId) {
    $.ajax({
        method: 'GET',
        url: '/Task/EditTask',
        data: { "taskId": taskId },
        dataType: 'html',
        success: function (result) {
            $('#edit-task-content').empty();
            $('#edit-task-content').append(result);
            ShowModal('modal-edit-task');
        }
    });
}
