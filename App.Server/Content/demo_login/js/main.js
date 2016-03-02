/* Height recalculate to window height */
$(function ()
{
    $('#login-column').css({ height: $(window).innerHeight() });
});
$(window).resize(function ()
{
    $('#login-column').css({ height: $(window).innerHeight() });
}
);  