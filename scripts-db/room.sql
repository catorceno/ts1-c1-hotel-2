/*
simple              15  100-114
doble individuales  15  115-129
doble matrimonial   12  130-141
suite               8   142-149
*/

-- Asegurate de que el hotel y los room_types ya fueron insertados
-- Este script asume que los IDs se generaron en orden; ajusta los UUIDs si usas uuid_generate_v4()

DO $$
DECLARE
  v_hotel_id      bigint;
  v_simple_id     bigint;
  v_suite_id      bigint;
  v_dbl_mat_id    bigint;
  v_dbl_ind_id    bigint;
  i               int;
BEGIN
  -- Recuperar IDs
  SELECT id INTO v_hotel_id   FROM hotel      WHERE name = 'Los Tajibos'         LIMIT 1;
  SELECT id INTO v_simple_id  FROM room_type  WHERE name = 'Simple'              LIMIT 1;
  SELECT id INTO v_suite_id   FROM room_type  WHERE name = 'Suite'               LIMIT 1;
  SELECT id INTO v_dbl_mat_id FROM room_type  WHERE name = 'Doble Matrimonial'   LIMIT 1;
  SELECT id INTO v_dbl_ind_id FROM room_type  WHERE name = 'Doble Individuales'  LIMIT 1;

  -- Simple: habitaciones 100-114 (15 hab.)
  FOR i IN 0..14 LOOP
    INSERT INTO room (hotel_id, type_id, number, current_status)
    VALUES (v_hotel_id, v_simple_id, 100 + i, 'available');
  END LOOP;

  -- Doble Individuales: habitaciones 115-129 (15 hab.)
  FOR i IN 0..14 LOOP
    INSERT INTO room (hotel_id, type_id, number, current_status)
    VALUES (v_hotel_id, v_dbl_ind_id, 115 + i, 'available');
  END LOOP;

  -- Doble Matrimonial: habitaciones 130-141 (12 hab.)
  FOR i IN 0..11 LOOP
    INSERT INTO room (hotel_id, type_id, number, current_status)
    VALUES (v_hotel_id, v_dbl_mat_id, 130 + i, 'available');
  END LOOP;

  -- Suite: habitaciones 142-149 (8 hab.)
  FOR i IN 0..7 LOOP
    INSERT INTO room (hotel_id, type_id, number, current_status)
    VALUES (v_hotel_id, v_suite_id, 142 + i, 'available');
  END LOOP;

END $$;