-- Work in progress



create extension dblink;


-- Select
select u."Name", u."ExternalId", p."name", p.ExternalId, ne."name", ne.ExternalId from "Users" as u, (
select * from dblink('host=******.postgres.database.azure.com port=5432 dbname==prod user=PostgreSQL password==****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT)
) as p,(
 select * from dblink('host=localhost port=5432 dbname=demo user==****** password==****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT)
) as ne
where u."Name" = p."name" and u."Name" = ne."name";


-- Update
-- After running this script, the entity framework cache needs to be cleared by restarting the application.
UPDATE "Users" AS u
SET "ExternalId" = ne.externalid 
from (
select * from dblink('host==******.postgres.database.azure.com port=5432 dbname=prod user==****** password==****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT)
) as p,(
 select * from dblink('host=localhost port=5432 dbname=demo user==****** password==****** options=-csearch_path=', $$select "Id", "Name", "DeletedBy", "ExternalId" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
AS T(Id UUID,
Name TEXT,
DeletedBy UUID,
ExternalId TEXT)
) as ne
where u."Name" = p."name" and u."Name" = ne."name";


SELECT dblink_disconnect('otherdb');

select * from "Users" u where u."DeletedBy" IS null