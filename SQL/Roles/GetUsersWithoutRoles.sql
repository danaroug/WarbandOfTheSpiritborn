-- GetUsersWithoutRoles.sql
-- Purpose: Find users who do not have any role assigned.
-- Output: One row per user without roles.

SELECT
    u.Id,
    u.Email
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
WHERE ur.UserId IS NULL
ORDER BY u.Email;
