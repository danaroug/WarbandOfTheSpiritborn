-- AssignRoleToUser.sql
-- Purpose: Assign any role to a user if it is not already assigned.
-- Usage: Set @Email and @RoleName before running.

DECLARE @Email NVARCHAR(256) = 'email';
DECLARE @RoleName NVARCHAR(256) = 'User';

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT
    u.Id,
    r.Id
FROM AspNetUsers u
INNER JOIN AspNetRoles r ON r.Name = @RoleName
WHERE u.Email = @Email
  AND NOT EXISTS
  (
      SELECT 1
      FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id
        AND ur.RoleId = r.Id
  );


