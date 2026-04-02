-- AssignAdministratorRoleToUser.sql
-- Assign the Administrator role to a specific user if not already assigned
-- Replace 'email' with the user's email before running
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
JOIN AspNetRoles r ON r.Name = 'Administrator'
WHERE u.Email = 'email'
  AND NOT EXISTS (
      SELECT 1 
      FROM AspNetUserRoles ur 
      WHERE ur.UserId = u.Id 
        AND ur.RoleId = r.Id
  );


