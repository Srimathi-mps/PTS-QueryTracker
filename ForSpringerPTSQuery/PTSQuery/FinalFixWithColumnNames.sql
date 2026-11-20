-- FINAL FIX WITH CORRECT COLUMN NAMES
-- The code expects columns named "FieldID" and "FieldName"
-- but the stored procedure returns "Cust_code" and "cust_sname"
-- We need to alias the columns to match what the code expects

USE [BooksItrack]
GO

-- ============================================================================
-- FIX: Update USP_TTS_GETCUSTOMERS to return FieldID and FieldName columns
-- ============================================================================
PRINT 'Fixing USP_TTS_GETCUSTOMERS column names...'
GO

/****** Object:  StoredProcedure [dbo].[USP_TTS_GETCUSTOMERS]    Script Date: 05-05-2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Updated to return columns with names that match what the C# code expects
ALTER PROCEDURE [dbo].[USP_TTS_GETCUSTOMERS]
    (
       @Cust_grname varchar(100) = NULL,
       @User_Role   int          = 0,
       @User_Code   varchar(50)  = NULL
    )    
AS     
BEGIN
    SET NOCOUNT ON;
    
    -- Role 2 = Admin/Full access - sees all customers
    IF (@User_Role = 2)
    BEGIN
        -- Admin sees all customers
        -- Return columns as FieldID and FieldName to match C# code expectations
        SELECT 
            Cust_code AS FieldID,      -- Alias to FieldID
            Cust_sname AS FieldName    -- Alias to FieldName
        FROM CUSTOMER 
        WHERE ISNULL(Cust_sname, '') != ''  -- Exclude empty names
        ORDER BY Cust_sname;
    END
    ELSE
    BEGIN
        -- For non-admin users, return only mapped customers
        -- First check if user has any mappings
        IF EXISTS (SELECT 1 FROM tbl_TTS_UserCustomerMapping WHERE UserCode = @User_Code)
        BEGIN
            -- User has mappings, return mapped customers
            -- Return columns as FieldID and FieldName to match C# code expectations
            SELECT DISTINCT 
                C.Cust_code AS FieldID,      -- Alias to FieldID
                C.Cust_sname AS FieldName    -- Alias to FieldName
            FROM Customer C
            INNER JOIN tbl_TTS_UserCustomerMapping PUM ON C.Cust_code = PUM.CustomerId
            WHERE PUM.UserCode = @User_Code
            AND ISNULL(C.Cust_sname, '') != ''  -- Exclude empty names
            ORDER BY C.Cust_sname;
        END
        ELSE
        BEGIN
            -- User has no mappings, return empty result set with correct column names
            -- This ensures the DataTable has the expected column structure even when empty
            SELECT 
                CAST(NULL AS INT) AS FieldID,
                CAST(NULL AS VARCHAR(100)) AS FieldName
            WHERE 1 = 0;  -- Always false, returns empty result set with correct schema
        END
    END
END
GO

PRINT 'USP_TTS_GETCUSTOMERS column names fixed.'
GO

-- ============================================================================
-- TEST THE FIX
-- ============================================================================
PRINT 'Testing the column name fix...'
GO

PRINT '1. Testing as ADMIN (role 2):'
EXEC [dbo].[USP_TTS_GETCUSTOMERS] @Cust_grname = NULL, @User_Role = 2, @User_Code = 'testadmin'

PRINT ''
PRINT '2. Testing as REGULAR USER with mappings (role 1):'
EXEC [dbo].[USP_TTS_GETCUSTOMERS] @Cust_grname = NULL, @User_Role = 1, @User_Code = 'MAC014393'

PRINT ''
PRINT '3. Column structure check:'
PRINT '   The result should have columns named FieldID and FieldName'
PRINT '   (not Cust_code and cust_sname)'
GO

-- ============================================================================
-- ALSO FIX USP_PTS_GETUSERROLEBYUSERNAME (if not already fixed)
-- ============================================================================
PRINT ''
PRINT 'Fixing USP_PTS_GETUSERROLEBYUSERNAME...'
GO

/****** Object:  StoredProcedure [dbo].[USP_PTS_GETUSERROLEBYUSERNAME]    Script Date: 05-05-2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Return only ROLEID as scalar value
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
END
GO

PRINT 'USP_PTS_GETUSERROLEBYUSERNAME fixed.'
GO

PRINT ''
PRINT 'FINAL FIXES COMPLETED!'
PRINT '======================'
PRINT 'Summary:'
PRINT '1. USP_TTS_GETCUSTOMERS now returns columns named FieldID and FieldName'
PRINT '2. USP_TTS_GETCUSTOMERS has proper role-based logic'
PRINT '3. USP_TTS_GETCUSTOMERS handles users with no customer mappings'
PRINT '4. USP_PTS_GETUSERROLEBYUSERNAME returns scalar ROLEID'
PRINT ''
PRINT 'This should fix the customer dropdown issue in the PTSQuery application.'
PRINT 'The C# code expects DataTable columns named FieldID and FieldName for binding.'
GO