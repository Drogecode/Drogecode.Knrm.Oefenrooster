-- Work in progress



create extension dblink;


-- Select
select u."Id",  u."Name" as myName, u."ExternalId" as myExternalid, u."Email" as myEmail, p."name", p.ExternalId, p.Email, ne."name", ne.ExternalId, ne.Email from "Users" as u, (select * from dblink('host=******.postgres.database.azure.com port=5432 dbname=prod user=PostgreSQL password=****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId", "Email" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT,
Email TEXT)
) as p,(
 select * from dblink('host=******.postgres.database.azure.com port=5432 dbname=demo user=****** password=****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId", "Email" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT,
Email TEXT)
) as ne
where u."Name" = p."name" and u."Name" = ne."name" and u."DeletedOn" is null and ne.Email NOT LIKE '%@kac.knrm.nl' and ne.Email not like '%@heiper.nl'
order by ne.Email desc;


-- Update
-- After running this script, the entity framework cache needs to be cleared by restarting the application.
UPDATE "Users" AS u
SET "ExternalId" = ne.externalid 
from (
select * from dblink('host=******.postgres.database.azure.com port=5432 dbname=prod user=****** password=****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId", "Email" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT,
Email TEXT)
) as p,(
 select * from dblink('host=******.postgres.database.azure.com port=5432 dbname=demo user=****** password=****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId", "Email" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT,
Email TEXT)
) as ne
where u."Name" = p."name" and u."Name" = ne."name" and u."DeletedOn" is null and ne.Email NOT LIKE '%@kac.knrm.nl' and ne.Email not like '%@heiper.nl';

update "Customers" set "TenantId" = '0237f806-c2b0-417b-9d23-eaec1e553223' where "TenantId" = 'd9754755-b054-4a9c-a77f-da42a4009365';
update "Customers" set "GroupId" = 'd52c4bba-9ba6-465a-9218-ed274f1e8f54' where "GroupId" = '1639c514-7c30-42d2-a384-795313b4eae1';
update "UserRoles" set "ExternalId" = 'd52c4bba-9ba6-465a-9218-ed274f1e8f54' where "ExternalId" = '1639c514-7c30-42d2-a384-795313b4eae1';
update "LinkUserRoles" set "SetExternal" = false where "SetExternal" = true;
update "LinkReportTrainingRoosterTraining" set "SetByExternalOn" = null where "SetByExternalOn" is not null;
update "ReportUsers" set "SetByExternalOn" = null where "SetByExternalOn" is not null;
update "ReportActions" set "SetByExternalOn" = null where "SetByExternalOn" is not null;
update "ReportTrainings" set "SetByExternalOn" = null where "SetByExternalOn" is not null;


-- Only when testing to break link with real calendar events. (different instance when moving but still important)
update "RoosterAvailable" set "CalendarEventId" = null;
delete from "UserPreComEvent";
delete from "UserSettings";

select * from "Users" u where u."DeletedBy" is not null order by "DeletedOn";
select * from "Users" u where u."Name" LIKE 'Jim%';
select * from "UserLogins" order by "LoginDate" desc;

select * from "Users" u where u."DeletedBy" is null and u."CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid
select * from "Users" u where u."Name" = 'Taco Droogers';
select * from "UserLogins" order by "LoginDate" desc;
