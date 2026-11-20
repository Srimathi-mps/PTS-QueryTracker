-- Fixed version of USP_TTS_GETCUSTOMERS stored procedure
-- Issues fixed:
-- 1. Consistent column names in both branches (returns Cust_code and cust_sname)
-- 2. Better role handling (role 2 = admin sees all, others see mapped customers)
-- 3. Returns Cust_sname (short name) consistently

USE [BooksItrack]
GO

/****** Object:  StoredProcedure [dbo].[USP_TTS_GETCUSTOMERS]    Script Date: 05-05-2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[USP_TTS_GETCUSTOMERS]
    (
       @Cust_grname varchar(100) = NULL,
       @User_Role   int          = 0,
       @User_Code   varchar(50)  = NULL
    )    
AS     
BEGIN
    SET NOCOUNT ON;
    
    -- Debug: Log parameters
    -- PRINT 'User_Role: ' + CAST(@User_Role AS VARCHAR(10)) + ', User_Code: ' + ISNULL(@User_Code, 'NULL');
    
    -- Role 2 = Admin/Full access - sees all customers
    IF (@User_Role = 2)
    BEGIN
        SELECT 
            Cust_code,
            Cust_sname  -- Use short name consistently
        FROM CUSTOMER 
        ORDER BY cust_sname;
    END
    ELSE
    BEGIN
        -- For non-admin users, return only mapped customers
        -- Using DISTINCT to avoid duplicates if user is mapped to same customer multiple times
        SELECT DISTINCT 
            C.Cust_code,
            C.Cust_sname  -- Use short name from Cust_sname column
        FROM Customer C
        INNER JOIN tbl_TTS_UserCustomerMapping PUM ON C.Cust_code = PUM.CustomerId
        WHERE PUM.UserCode = @User_Code
        ORDER BY C.Cust_sname;
        
        -- Debug: If no rows returned, check if user has mappings
        -- IF @@ROWCOUNT = 0
        -- BEGIN
        --     PRINT 'No customer mappings found for user: ' + @User_Code;
        --     PRINT 'Check tbl_TTS_UserCustomerMapping table for UserCode: ' + @User_Code;
        -- END
    END
END
GO

-- Test the stored procedure
/*
-- Test as admin (role 2)
EXEC [dbo].[USP_TTS_GETCUSTOMERS] @Cust_grname = NULL, @User_Role = 2, @User_Code = 'testadmin'

-- Test as regular user (role 1 or other)
EXEC [dbo].[USP_TTS_GETCUSTOMERS] @Cust_grname = NULL, @User_Role = 1, @User_Code = 'MAC014393'

-- Test with no mappings
EXEC [dbo].[USP_TTS_GETCUSTOMERS] @Cust_grname = NULL, @User_Role = 1, @User_Code = 'testuser'
*/
GO