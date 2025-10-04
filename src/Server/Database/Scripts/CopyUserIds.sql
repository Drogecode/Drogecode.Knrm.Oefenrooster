-- Work in progress

create extension dblink;

select u."Name", p."name" from "Users" as u, (

    select * from dblink('host=localhost port=5432 dbname=BackupProdHui user=DemoAdmin password=[replace with passwoord] options=-csearch_path=', $$select "Id", "Name", "DeletedBy" from public."Users" 
WHERE "DeletedBy" IS null AND "CustomerId"::uuid = 'd9754755-b054-4a9c-a77f-da42a4009365'::uuid$$)
                      AS T(Id UUID,
                           Name TEXT,
                           DeletedBy UUID)

) as p

where u."Name" = p."name";