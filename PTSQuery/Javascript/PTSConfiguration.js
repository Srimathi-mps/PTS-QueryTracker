function ConfigValidation() {

    var stage = document.getElementById('StageDropDown').value;
    var customer = document.getElementById('CustomerDropDown').value;
    var type = document.getElementsByName('rdoType');


    
    var flag = true;
    var msg = ''
    
    if (stage == "0") {
        flag = false;
        msg = "Select stage \n";
    }

    if (customer == "0") {
        flag = false;
        msg = msg + "Select customer \n";
    }

    if (type[1].checked) {
        var signalFileName = document.getElementById('SignalFileNameTextBox').value;
        var signalFileDestination = document.getElementById('SignalFileDestinationTextBox').value;
        var zipFileName = document.getElementById('ZipFileNameTextBox').value;
        var zipDestination = document.getElementById('ZipFileDestinationTextBox').value;


        if (trim(signalFileName) == "") {
            flag = false;
            msg = msg + "Signal file name is blank \n";
        }

        if (trim(zipFileName) == "") {
            flag = false;
            msg = msg + "Zip file name is blank \n";
        }

        if (signalFileName.indexOf("<") > 0 || signalFileName.indexOf(">") > 0) {
            flag = false;
            msg = msg + "'<' or '>' symbols are not allowed in signal file name \n";
        }

        if (zipFileName.indexOf("<") > 0 || zipFileName.indexOf(">") > 0) {
            flag = false;
            msg = msg + "'<' or '>' symbols are not allowed in zip file name \n";
        }

        if (trim(signalFileDestination) == "") {
            flag = false;
            msg = msg + "Signal file destination is blank \n";
        }

        if (trim(zipDestination) == "") {
            flag = false;
            msg = msg + "Zip file destination is blank \n";
        }
    }
    else {
        var signalFileName = document.getElementById('txtFromEmail').value;
        var signalFileDestination = document.getElementById('txtToEmail').value;
        var zipFileName = document.getElementById('txtEmailSubject').value;
        var zipDestination = document.getElementById('txtEmailContent').value;


        if (trim(signalFileName) == "") {
            flag = false;
            msg = msg + "From EmailID is blank \n";
        }

        if (trim(signalFileDestination) == "") {
            flag = false;
            msg = msg + "To EmailID is blank \n";
        }

        if (trim(zipFileName) == "") {
            flag = false;
            msg = msg + "Email Subject is blank \n";
        }

        if (trim(zipDestination) == "") {
            flag = false;
            msg = msg + "EmailContent is blank \n";
        }
    }
    

    if (flag == false) {
        alert(msg);
        return flag;
    }
    else
        return flag;
}

function ConfirmDelete() {
    var result = confirm("Are you sure you want to delete?");

    return result;
}