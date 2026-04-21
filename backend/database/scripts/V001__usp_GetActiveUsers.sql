USE [ViamaticaDB];
GO

CREATE OR ALTER PROCEDURE dbo.usp_GetActiveUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.userid,
        u.username,
        u.email,
        r.rolname AS role_name,
        us.description AS status_description
    FROM dbo.usertable AS u
    INNER JOIN dbo.rol AS r ON r.rolid = u.rol_rolid
    INNER JOIN dbo.userstatus AS us ON us.statusid = u.userstatus_statusid
    WHERE u.isdeleted = 0
      AND u.userstatus_statusid = 'ACT'
      AND u.userapproval = 1;
END;
GO
