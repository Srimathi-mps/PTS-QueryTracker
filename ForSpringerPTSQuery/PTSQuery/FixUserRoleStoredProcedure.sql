-- Fixed version of USP_PTS_GETUSERROLEBYUSERNAME stored procedure
-- Issue: The original returns multiple columns but code expects ROLEID as first column
-- Two options: 
-- Option 1: Change stored procedure to return only ROLEID
-- Option 2: Change calling code to read ROLEID column

USE [BooksItrack]
GO

/****** Object:  StoredProcedure [dbo].[USP_PTS_GETUSERROLEBYUSERNAME]    Script Date: 05-05-2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- OPTION 1: Return only ROLEID (simpler for calling code)
ALTER PROCEDURE [dbo].[USP_PTS_GETUSERROLEBYUSERNAME]
	@USERNAME VARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON
 	
	-- Return only the ROLEID as a scalar value
	SELECT UM.ROLEID 
	FROM TBL_PTS_USERROLEMAPPING UM  
	INNER JOIN LKP_PTS_USERROLE UR ON UM.ROLEID = UR.ROLEID  
	WHERE USERNAME = @USERNAME
	
	-- If user not found, return NULL (caller should handle this)
END
GO

-- OPTION 2: Keep returning all columns but update calling code to read ROLEID column
-- (This would require changing the C# code to read the ROLEID column instead of using ExecuteScalar)
/*
ALTER PROCEDURE [dbo].[USP_PTS_GETUSERROLEBYUSERNAME]
	@USERNAME VARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON
 	
	SELECT MAPPINGID, USERNAME, UM.ROLEID, UR.ROLENAME 
	FROM TBL_PTS_USERROLEMAPPING UM  
	INNER JOIN LKP_PTS_USERROLE UR ON UM.ROLEID = UR.ROLEID  
	WHERE USERNAME = @USERNAME
END
GO
*/

-- Test the stored procedure
/*
-- Test with existing user
EXEC [dbo].[USP_PTS_GETUSERROLEBYUSERNAME] @USERNAME = 'MAC014393'

-- Test with non-existent user (should return empty result set)
EXEC [dbo].[USP_PTS_GETUSERROLEBYUSERNAME] @USERNAME = 'nonexistent'
*/
GO