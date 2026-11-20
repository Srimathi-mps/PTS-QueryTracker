function ValidateForm()
{
    var frm=document.form1; 
    var flag=true;
    var msg="";
    
    if (frm.UserNameTextbox.value=="")
    {
        flag=false;
        msg="Enter username \n" ;
    }
    if (frm.RoleDropDown.value=="0")
    {
        flag=false;
        msg=msg+"Select role \n" ;
    }
    
    if (flag==false)
    {
        alert(msg);
    }
    return flag;
}

function ViewUserRole()
{
    var frm=document.form1; 

    flag=false ;
    index=-1;
    tempcode="";

    radbtnexist=document.getElementById ("radbtn");
    if (radbtnexist==null )
        return ;
    if (frm.radbtn.length>1)
    {
        for(i=0;i<frm.radbtn.length;i++ )
        {
            if(frm.radbtn[i].checked==true )
            {
                index =i;
                flag=true;
                tempcode =frm.radbtn[i].value;
                break;
           }
        }
    }
    else 
    {
        if (frm.radbtn.checked==true)
        {
            index=0;
            flag =true;
            tempcode =frm.radbtn.value;
        }  
    }
    if (index ==-1)
    {
        alert("select any value");
        frm.MappingIDHidden.value="";
        return;
    } 
    else
    {
        var arr=tempcode.split('~');
        
        //alert(tempcode);
        if (arr.length>0)
        {
            frm.MapButton.disabled=true;
            frm.UpdateButton.disabled=false ;

            frm.MappingIDHidden.value=arr[0];
            frm.UserNameTextbox.value=arr[1];
            frm.RoleDropDown.value=arr[2];
        }
    }
}