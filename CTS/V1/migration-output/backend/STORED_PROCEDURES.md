# Stored Procedures Reference

This document maps the original ASPX service calls to the actual database stored procedures.

## Dropdowns

### Customers
**SP:** `USP_TTS_GETCUSTOMERS`
```sql
EXEC USP_TTS_GETCUSTOMERS 
  @Cust_grname = '',
  @User_Role = 0,
  @User_Code = 'MAC014393'
```
**Returns:** `Cust_code`, `cust_sname`

### Journals
**SP:** `USP_TTS_GetAllJournal`
```sql
EXEC USP_TTS_GetAllJournal 
  @jour_ccode = 163,
  @case = 'ALL'
```
**Returns:** `jour_code`, `jour_sname`

### Articles (Jobs)
**SP:** `usp_PTS_GetWIPArticles`
```sql
EXEC usp_PTS_GetWIPArticles 
  @customerCode = 163,
  @journalCode = 1,
  @Volume = '',
  @Issue = ''
```
**Returns:** `FIELDID`, `FIELDNAME`

### Departments
**SP:** `USP_QMS_GetDepartment`
```sql
EXEC USP_QMS_GetDepartment
```
**Returns:** `fieldId`, `fieldName`

## Barcode Validation

### Validate Customer/Journal/Article
**SP:** `usp_PTS_ValidateCustomerJournalArticle`
```sql
DECLARE @errorMessage NVARCHAR(500)
EXEC usp_PTS_ValidateCustomerJournalArticle 
  @custFName = 'CustomerName',
  @journalName = 'JournalName',
  @article = 'ART001',
  @errorMessage = @errorMessage OUTPUT
SELECT @errorMessage
```

### Get Customer/Journal Codes
**SP:** `USP_TTS_GETJOURNALDETAILSBYJOURNALCODE` (used in reverse — query by name)
Or use direct table query.

### Get Stage by Article
**SP:** `usp_PTS_GetStageByCustomerJournalArticle`
```sql
EXEC usp_PTS_GetStageByCustomerJournalArticle 
  @customerCode = 163,
  @journalCode = 1,
  @article = 'ART001',
  @Volumn = '',
  @issue = ''
```

## ISBN
**SP:** `usp_Contrast_GetIsbn`
```sql
EXEC usp_Contrast_GetIsbn @bookid = 1
```
**Returns:** `Jour_issn_no`

## Queries (Grid)

### Get Opened Requests
**SP:** `usp_PTS_GetPTSProblemsBy_Article`
```sql
EXEC usp_PTS_GetPTSProblemsBy_Article 
  @customerCode = 163,
  @journalCode = 1,
  @article = 'ART001,ART002',
  @SubStageID = 1
```
**Returns:** All query fields including `PTId`, `PTCode`, `Article`, `PTSRemarks`, `Status`, `QueryType`, `ResolvedRemarks`, etc.

### Update Query
**SP:** `USP_deletequerynew`
```sql
EXEC USP_deletequerynew 
  @ID = 123,
  @Reamarks = 'Updated remarks',
  @Response = 'Updated response',
  @type = 'update'
```

### Delete Query
**SP:** `USP_deletequery`
```sql
EXEC USP_deletequery 
  @ID = 123,
  @Reamarks = '',
  @type = 'delete'
```

### Approve Instruction
**SP:** `USP_PTSUpdatequeryNew`
```sql
EXEC USP_PTSUpdatequeryNew 
  @ID = 123,
  @Proceed_Req = 1,
  @type = 'UpdateApprove'
```

### Check Approve Visible
**SP:** `USP_PTSUpdatequeryNew`
```sql
EXEC USP_PTSUpdatequeryNew 
  @ID = 123,
  @Proceed_Req = NULL,
  @type = 'GetInitiateFile'
```
**Returns:** 'success' if already approved

## Role
**SP:** `USP_PTS_GETUSERROLEBYUSERNAME`
```sql
EXEC USP_PTS_GETUSERROLEBYUSERNAME @USERNAME = 'MAC014393'
```
**Returns:** RoleID (scalar)

## Problem Types
**SP:** `USP_PTS_GetProblemTypeDetails`
```sql
EXEC USP_PTS_GetProblemTypeDetails 
  @CustomerCode = 163,
  @JournalCode = '1',
  @JobChapterID = 'ART001',
  @StageID = 1,
  @Volume = '',
  @Issue = ''
```
**Returns:** Problem type list

## Create Query
**SP:** `usp_PTS_InsUpd_PTSProblems`
```sql
DECLARE @insertStatus VARCHAR(50)
EXEC usp_PTS_InsUpd_PTSProblems 
  @SubstageID = 1,
  @stage = 'S100',
  @customerCode = 163,
  @journalCode = 1,
  @article = 'ART001',
  @DepartmentId = 1,
  @volume = '',
  @issue = '',
  @ptsRemarks = 'Query remarks',
  @taskclosed = 0,
  @userCode = 'MAC014393',
  @problemTypeCode = '1',
  @zipFileUpload = 0,
  @querySavedBy = 'MAC014393',
  @queryClosedBy = '',
  @msgID = '',
  @ZipFilePath = '',
  @Sequence = '',
  @RoundCount = 0,
  @FILECONTENT = '',
  @insertStatus = @insertStatus OUTPUT
SELECT @insertStatus
```

## Resolve Query
**SP:** `USP_PTS_RESOLVE_PTSPROBLEM_BYID`
```sql
DECLARE @STATUS VARCHAR(50)
EXEC USP_PTS_RESOLVE_PTSPROBLEM_BYID 
  @ID = 123,
  @PTSQueryResolveRemarks = 'Resolved',
  @ResolvedBy = 'MAC014393',
  @STATUS = @STATUS OUTPUT
SELECT @STATUS
```

## Stage Details
**SP:** `usp_PTS_GetSubStageDetails`
```sql
EXEC usp_PTS_GetSubStageDetails 
  @Art_Ccode = 163,
  @Art_Jcode = 1,
  @Art_Acode = 'ART001'
```

## Round Count
**SP:** `USP_PTS_GetWIPRoundCountByArticle`
```sql
EXEC USP_PTS_GetWIPRoundCountByArticle 
  @CustCode = 163,
  @JourCode = 1,
  @Article = 'ART001'
```
