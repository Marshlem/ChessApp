CREATE OR REPLACE FUNCTION delete_opening_node_subtree(
    p_user_id UUID,
    p_opening_id UUID,
    p_node_id UUID
)
RETURNS VOID
LANGUAGE sql
AS $$
    WITH RECURSIVE subtree AS (
        SELECT n."Id"
        FROM "OpeningNodes" n
        JOIN "Openings" o
          ON o."Id" = n."OpeningId"
         AND o."UserId" = p_user_id
        WHERE n."Id" = p_node_id
          AND n."OpeningId" = p_opening_id

        UNION ALL

        SELECT c."Id"
        FROM "OpeningNodes" c
        JOIN subtree s ON s."Id" = c."ParentNodeId"
    )
    DELETE FROM "OpeningNodes"
    WHERE "Id" IN (SELECT "Id" FROM subtree);
$$;
