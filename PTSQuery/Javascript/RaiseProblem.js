

function ValidateBarCode() 
{
    //var barCode = document.getElementById('<%=BarcodeTextbox.ClientID%>').value;
    var barCode = document.getElementById('BarCodeTextBox').value;
    var mySplitResult = barCode.split("|");
    var flag = true;

    //alert(mySplitResult.length)
    
    if ((mySplitResult.length != 3) && (mySplitResult.length != 4)) 
    {
        flag=false;        
    }

    for (i = 0; i < mySplitResult.length; i++) {
        if (trim(mySplitResult[i]) == '') {
            flag = false;
            break;
        }

    }

    if (mySplitResult.length == 4) 
    {
        var volume = mySplitResult[2];
        var issue = mySplitResult[3];
        
        //if vol/issue has "-" then no alphabet is allowed in vol/issue

        if (volume.indexOf('-') > 0) 
        {
            var volArr = volume.split('-');

            if (volArr[0].indexOf('.') > 0) {
                flag = false;
            }

            if (volArr[1].indexOf('.') > 0) {
                flag = false;
            }
            
            if (!isNumber(volArr[0]))
                flag = false;
            
            if (!isNumber(volArr[1]))
                flag = false;
        } //end volume format checking

        if (issue.indexOf('-') > 0) 
        {
            var issueArr = issue.split('-');

            if (issueArr[0].indexOf('.') > 0) {
                flag = false;
            }

            if (issueArr[1].indexOf('.') > 0) {
                flag = false;
            }

            //if (!isNumber(issueArr[0]))
            //    flag = false;

            //if (!isNumber(issueArr[1]))
            //    flag = false;
        } //end volume format checking
    }

    if (flag == false) {
        alert("Invalid Barcode");
        return flag;
    }
    else
        return flag;

}

function isNumber(n) {
    return !isNaN(parseInt(n)) && isFinite(n);
}


function ValidationForSaveAndClose() {
    var customerCode = document.getElementById('CustomerDropDown').value;
    var journalCode = document.getElementById('JournalDropDown').value;
    var stage = document.getElementById('StageDropDown').value;
    var jobType = document.getElementById('JobTypeHidden').value;
    var ptsRemarks = document.getElementById('PTDescriptionTextBox').value;
//    var ptsAdd = document.getElementById('ndnPtsAddValidate').value;
    
    var flag = true;
    var msg = ''

    //alert(journalCode);
    if (stage == "0") {
        flag = false;
        msg = "Select stage \n";
    }

    if (ptsRemarks == "") {
        flag = false;
        msg = "Please add atleast one problemtype \n";
    }

//    if (ptsAdd == "0") {
//        flag = false;
//        msg = "Please add atleast one problemtype for save.\n";
//    }

    if (customerCode == "0") {
        flag = false;
        msg = msg + "Select customer \n";
    }

    if (journalCode == "0") {
        flag = false;
        msg = msg + "Select journal \n";
    }

    if (jobType == "1") {
    //if (document.getElementById('VolumeDropDown') != 'null') {
    //    if (document.getElementById('ArticleDropDown').selectedIndex > 0) {
            var article = document.getElementById('ArticleDropDown').value;
            if (article == "0") {
                flag = false;
                msg = msg + "Select article \n";
            }
    //    }
    }

    if (jobType == "2") {
    //if (document.getElementById('VolumeDropDown')!= 'null' && document.getElementById('IssueDropDown') != 'null') {
    //    if (document.getElementById('VolumeDropDown').selectedIndex > 0 && document.getElementById('IssueDropDown').selectedIndex > 0) {
            var volume = document.getElementById('VolumeDropDown').value;
            var issue = document.getElementById('IssueDropDown').value;

            if (volume == "0") {
                msg = msg + "Select volume \n";
                flag = false;
            }

            if (issue == "0") {
                flag = false;
                msg = msg + "Select issue \n";
            }
    //    }
    }
//    var problemType = document.getElementById('ProblemTypeDropDown').value;
//    if (problemType == "0") {
//        flag = false;
//        msg = msg + " Select problemType \n";
//    }

    if (flag == false) {
        alert(msg);
        return flag;
    }
    else
        return flag;
}

function ValidationForSaveAndCloseNew() {
    var customerCode = customerCodeT;
    var journalCode = journalCodeT;
    var stage = stageIDT;
    var jobType = jobTypeT;
    var ptsRemarks = 'TST';//document.getElementById('PTDescriptionTextBox').value;
////    var ptsAdd = document.getElementById('ndnPtsAddValidate').value;
    
    var flag = true;
    var msg = ''

    //alert(journalCode);
    if (stage == "0") {
        flag = false;
        msg = "Select stage \n";
    }

//    if (ptsRemarks == "") {
//        flag = false;
//        msg = "Please add atleast one problemtype \n";
//    }

////    if (ptsAdd == "0") {
////        flag = false;
////        msg = "Please add atleast one problemtype for save.\n";
////    }

    if (customerCode == "0") {
        flag = false;
        msg = msg + "Select customer \n";
    }

    if (journalCode == "0") {
        flag = false;
        msg = msg + "Select journal \n";
    }

    if (jobType == "1") {
    ////if (document.getElementById('VolumeDropDown') != 'null') {
    ////    if (document.getElementById('ArticleDropDown').selectedIndex > 0) {
            var article = ArticleT; ////document.getElementById('ArticleDropDown').value;
            if (article == "0") {
                flag = false;
                msg = msg + "Select article \n";
            }
    ////    }
    }

    if (jobType == "2") {
    //if (document.getElementById('VolumeDropDown')!= 'null' && document.getElementById('IssueDropDown') != 'null') {
    //    if (document.getElementById('VolumeDropDown').selectedIndex > 0 && document.getElementById('IssueDropDown').selectedIndex > 0) {
            var volume = VolumeT;////document.getElementById('VolumeDropDown').value;
            var issue = IssueT;////document.getElementById('IssueDropDown').value;

            if (volume == "0") {
                msg = msg + "Select volume \n";
                flag = false;
            }

            if (issue == "0") {
                flag = false;
                msg = msg + "Select issue \n";
            }
    //    }
    }
//    var problemType = document.getElementById('ProblemTypeDropDown').value;
//    if (problemType == "0") {
//        flag = false;
//        msg = msg + " Select problemType \n";
//    }

    if (flag == false) {
        alert(msg);
        return flag;
    }
    else
        return flag;
}



function ZipFiles() {
    debugger;
    alert('sathish');
    if (ValidationForSaveAndClose()) {
        var customerCode = document.getElementById('CustomerDropDown').value;
        var journalCode = document.getElementById('JournalDropDown').value;
        var stageID = document.getElementById('StageDropDown').value;
        var jobType = document.getElementById('JobTypeHidden').value;
        var username = document.getElementById('hdnUserName').value;
        var ddlStage = document.getElementById('StageDropDown');
        var stageName = ddlStage.options[ddlStage.selectedIndex].text;
        var ProblemTypeDropDown = document.getElementById('ProblemTypeDropDown');
        var ProblemTypeId = ProblemTypeDropDown.options[ProblemTypeDropDown.selectedIndex].value;

        var ZipFilePath = document.getElementById('hfZipFilePath').value;
        //var ZipFilePath = document.getElementById("txtZipFilePath").value;
        
        
//        var lbtnZip = document.getElementById('lbtnZipFile');
        //alert(stageName);


//        if (document.getElementById('ArticleDropDown').selectedIndex > 0) {
//            // updated by prasanta
//            //article = document.getElementById('ArticleDropDown').value;
//            article = document.getElementById('ArticleDropDown').item(document.getElementById('ArticleDropDown').selectedIndex).text
//        }

//        if (document.getElementById('VolumeDropDown').selectedIndex > 0 && document.getElementById('IssueDropDown').selectedIndex > 0) {
//            var volume = document.getElementById('VolumeDropDown').value;
//            var issue = document.getElementById('IssueDropDown').value;

//            article = volume + "~" + issue;
//        }



        if (jobType == "1") {
            // updated by prasanta
            //article = document.getElementById('ArticleDropDown').value;
            article = document.getElementById('ArticleDropDown').item(document.getElementById('ArticleDropDown').selectedIndex).text
        }

        if (jobType == "2") {
            var volume = document.getElementById('VolumeDropDown').value;
            var issue = document.getElementById('IssueDropDown').value;

            article = volume + "~" + issue;
        }

        //window.open("ZipFiles.aspx?customerCode=" + customerCode + "&journalCode=" + journalCode + "&article=" + article + "&stageID=" + stageID +"&stageName=" + stageName + "&userName=" + username + "&jobType=" + jobType + "&ProblemTypeId=" + ProblemTypeId, "zip", "scrollbars=yes,resizable=no,width=600,height=500");
        window.open("ZipFiles.aspx?customerCode=" + customerCode + "&journalCode=" + journalCode + "&article=" + article + "&stageID=" + stageID + "&stageName=" + stageName + "&userName=" + username + "&jobType=" + jobType + "&ProblemTypeId=" + ProblemTypeId + "&ZipFilePath=" + ZipFilePath, "zip", "scrollbars=yes,resizable=no,width=600,height=500");
    }
    else
    { return false; }

}

function ZipFilesNew() {
    

    if (ValidationForSaveAndCloseNew()) {
        var customerCode = customerCodeT;
        var journalCode = journalCodeT;
        var stageID = stageIDT;
        var jobType = jobTypeT;
        var username = usernameT;
        var stageName = stageNameT;
        var ProblemTypeId = ProblemTypeIdT;
        var hfZipFilePath = document.getElementById('hfZipFilePath');
        var ZipFilePath = hfZipFilePath.value;
        
        if (jobType == "1") 
            article = ArticleT
        
        if (jobType == "2") {
            var volume = VolumeT;
            var issue = IssueT;
            article = volume + "~" + issue;
        }
        window.open("ZipFiles.aspx?customerCode=" + customerCode + "&journalCode=" + journalCode + "&article=" + article + "&stageID=" + stageID + "&stageName=" + stageName + "&userName=" + username + "&jobType=" + jobType + "&ProblemTypeId=" + ProblemTypeId + "&ZipFilePath=" + ZipFilePath, "zip", "scrollbars=yes,resizable=no,width=600,height=500");
    }
    else
    { return false; }

}


function ValidateListView() {
    var listView = null;
    var isValid = false;

    listView = document.getElementById('ctl00_head_IssueApprovalListView_ArticlesTable');
    //alert(listView.rows.length);

    for (var i = 1; i < listView.rows.length; i++) {
        var chkboxName = 'ctl00_head_IssueApprovalListView_ctrl' + (i - 1) + '_LVCheckBox';
        var input = document.getElementById(chkboxName);
        if (input != null) {
            if (input.type == "checkbox") {
                if (input.checked) {
                    isValid = true;
                    break;
                }
            }
        }

    }
    if (isValid) {
        return isValid;
    }
    else {
        alert("Select atleast one check box");
        return isValid;
    }

}

