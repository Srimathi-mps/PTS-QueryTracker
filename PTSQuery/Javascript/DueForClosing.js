function ZipFiles(customerCode, journalCode, article, stageID, stageName, username, jobType, ProblemTypeId, ZipFilePath) {
    //window.open("ZipFiles.aspx?customerCode=" + customerCode + "&journalCode=" + journalCode + "&article=" + article + "&stageID=" + stageID + "&stageName=" + stageName + "&userName=" + username + "&jobType=" + jobType + "&ProblemTypeId=" + ProblemTypeId, "zip", "scrollbars=yes,resizable=no,width=600,height=500");
    window.open("ZipFiles.aspx?customerCode=" + customerCode + "&journalCode=" + journalCode + "&article=" + article + "&stageID=" + stageID + "&stageName=" + stageName + "&userName=" + username + "&jobType=" + jobType + "&ProblemTypeId=" + ProblemTypeId + "&ZipFilePath=" + ZipFilePath, "zip", "scrollbars=yes,resizable=no,width=600,height=500");
}

function ValidateGridViewCheckBoxes() 
{
    var valid = false;
    var gView = document.getElementById("GVDueArticles");

    for (var i = 0; i < gView.all.length; i++) 
    {
        var node = gView.all[i];
        if (node != null && node.type == "checkbox" && node.checked) 
        {
            valid = true;
            break;
        }
    }
    if (!valid)
    {
        alert("Select atleast one article!");
    }
    return valid;
}
    
function ConfirmDelete()
{
    var rFlag=false;
    if (ValidateGridViewCheckBoxes())
    {
        rFlag=confirm("Are you sure to delete the selected problems");
    }
    return rFlag;
}