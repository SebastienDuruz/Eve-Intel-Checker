/**
 * Autor : Sébastien Duruz
 * Date : 09.06.2023
 * Description : Misc scripts for EveIntelChecker
 */

window.addEventListener('load', function() {
    disableScroll();
})

function preventScroll(e){
    e.preventDefault();
    e.stopPropagation();

    return false;
}

function disableScroll() {
    document.querySelector('.noScrollContainer').addEventListener('wheel', preventScroll);
}

function setContentHeight() {
    
}
