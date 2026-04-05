-- CheckUserRoles.sql
-- Purpose: Check which roles a specific user has.
-- Usage: Set @Email before running the query.
-- Output: Lists UserId, RoleId, RoleName, and Email. Each role appears in a separate row.

DECLARE @Email NVARCHAR(256) = 'email';

SELECT ur.UserId, ur.RoleId, r.Name AS RoleName, u.Email
FROM AspNetUserRoles ur
JOIN AspNetRoles r ON ur.RoleId = r.Id
JOIN AspNetUsers u ON ur.UserId = u.Id
WHERE u.Email = @Email;



