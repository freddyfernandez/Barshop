$(function () {


    let equipo = $('#equipo').offset().top;
    let servicio = $('#servicio').offset().top;
    let trabajo = $('#trabajo').offset().top;

    window.addEventListener('resize', function () {
        let equipo = $('#equipo').offset().top;
        let servicio = $('#servicio').offset().top;
        let trabajo = $('#trabajo').offset().top;
    


    });
   
    $('#enlace-equipo').on('click', function (e) {
        e.preventDefault();
        $('html,body').animate({ scrollTop: equipo - 88 }, 600);

    });
    $('#enlace-servicio').on('click', function (e) {
        e.preventDefault();
        $('html,body').animate({ scrollTop: servicio - 88 }, 600);

    });
    $('#enlace-trabajo').on('click', function (e) {
        e.preventDefault();
        $('html,body').animate({ scrollTop: trabajo - 88 }, 600);

    });




});