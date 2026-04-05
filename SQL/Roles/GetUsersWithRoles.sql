-- GetUsersWithRoles.sql
-- Purpose: Show all users with a comma-separated list of their roles.
-- Output: One row per user.

SELECT
    u.Email,
    COALESCE(STRING_AGG(r.Name, ', '), 'No roles assigned') AS Roles
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
GROUP BY u.Email
ORDER BY u.Email;
