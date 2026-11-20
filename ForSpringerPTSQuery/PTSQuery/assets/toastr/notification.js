function success(msg, type) {
    
    if (type == 'success') {
     
        toastr.success(msg);
 setTimeout(function () {
            
        }, 2000);
    }
    if (type == 'warning') {
        toastr.warning(msg);
    }
    if (type == 'info') {
        toastr.info(msg);
    }
    if (type == 'error') {
        toastr.error(msg);
    }
}


function successRedirect(msg, type, pageurl) {
    if (type == 'success') {

        toastr.success(msg);
        setTimeout(function () {
            window.location = pageurl;
        }, 2000);

    }
    if (type == 'warning') {
        toastr.warning(msg);
    }
    if (type == 'info') {
        toastr.info(msg);
    }
    if (type == 'error') {
        toastr.error(msg);
    }
}


