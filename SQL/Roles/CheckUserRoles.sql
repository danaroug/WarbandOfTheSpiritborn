-- CheckUserRoles.sql
-- Purpose: Check which roles a specific user has.
-- Usage: Replace 'danairougkeri@outlook.com' with the email of the user you want to check.
-- Output: Lists UserId, RoleId, RoleName, and Email. Each role appears in a separate row.
SELECT ur.UserId, ur.RoleId, r.Name AS RoleName, u.Email
FROM AspNetUserRoles ur
JOIN AspNetRoles r ON ur.RoleId = r.Id
JOIN AspNetUsers u ON ur.UserId = u.Id
WHERE u.Email = 'email';



