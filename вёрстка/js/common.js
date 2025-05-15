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

function ShowModal(element_id){
    document.getElementById(element_id).style.display = "flex";
}

function CloseModal(element_id){
    document.getElementById(element_id).style.display = "none";
}



