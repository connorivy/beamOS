CREATE TABLE IF NOT EXISTS "users" (
  "id" uuid PRIMARY KEY NOT NULL,
  "name" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "models" (
  "id" uuid PRIMARY KEY NOT NULL,
  "name" text NOT NULL,
  "description" text NOT NULL DEFAULT ''
);
