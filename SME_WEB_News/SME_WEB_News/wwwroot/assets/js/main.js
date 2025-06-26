$(document).ready(function () {
    $('.cate-filter .btn-fiter-item').on('click', function () {
        // button active //
        $(".cate-filter .btn-fiter-item").removeClass('active');
        $(this).addClass('active');

        // Category Item //
        selectedClass = $(this).attr("data-rel");
        if (selectedClass == "all") {
            $('.card-item').attr('style', 'display:flex;');
        } else {
            $('.card-item').attr('style', 'display:none;');
            $('.card-item.' + selectedClass).attr('style', 'display:flex;');
        }

    });

    $('.cate-filter-mb a').on('click', function () {
        // Category Item //
        selectedClass = $(this).attr("data-rel");
        if (selectedClass == "all") {
            $('.card-item').attr('style', 'display:flex;');
        } else {
            $('.card-item').attr('style', 'display:none;');
            $('.card-item.' + selectedClass).attr('style', 'display:flex;');
        }
    });


})