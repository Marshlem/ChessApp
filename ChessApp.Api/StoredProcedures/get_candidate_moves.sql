CREATE OR REPLACE FUNCTION get_candidate_moves(
    p_user_id UUID,
    p_opening_id UUID,
    p_parent_node_id UUID
)
RETURNS TABLE (
    node_id UUID,
    parent_node_id UUID,
    move_san TEXT,
    fen TEXT,
    line_type INT,
    comment TEXT,
    trained_count INT,
    failed_count INT,
    last_trained_at_utc TIMESTAMPTZ,
    next_due_at_utc TIMESTAMPTZ,
    created_at_utc TIMESTAMPTZ
)
LANGUAGE sql
AS $$
    SELECT
        n."Id",
        n."ParentNodeId",
        n."MoveSan",
        n."Fen",
        n."LineType",
        n."Comment",
        COALESCE(s."TrainedCount", 0),
        COALESCE(s."FailedCount", 0),
        s."LastTrainedAtUtc",
        s."NextDueAtUtc",
        n."CreatedAtUtc"
    FROM "OpeningNodes" n
    JOIN "Openings" o
        ON o."Id" = n."OpeningId"
       AND o."UserId" = p_user_id
    LEFT JOIN "TrainingNodeStats" s
        ON s."OpeningNodeId" = n."Id"
       AND s."UserId" = p_user_id
    WHERE n."OpeningId" = p_opening_id
      AND n."ParentNodeId" = p_parent_node_id
    ORDER BY n."CreatedAtUtc";
$$;
