-- Migration: add 42 new plants from NPRC May 2026 dataset
-- Generated: 2026-05-11
-- Source: NPRC May 2026 spreadsheet (42 new plants)
-- Idempotent on ScientificName; safe to re-run.
--
-- IMPORTANT post-apply step: restart pac.service (and pac-stage.service on stage)
-- to invalidate the static PlantCrawler._cachedPlantLookup. Without a restart, the
-- crawler will not recognize the new ScientificName/CommonName entries until the
-- next process restart.

-- ASCI5  Asclepias cinerea  (Carolina milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'fd774548-87fc-57a5-ba5a-7352db718fa5' AS `Id`,
  'ASCI5' AS `Symbol`,
  'Asclepias cinerea' AS `ScientificName`,
  'Carolina milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May - Sep' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias cinerea'
);

-- ASCO15  Asclepias connivens  (largeflower milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '28498920-67b3-553d-a0a3-9bb71637ecb1' AS `Id`,
  'ASCO15' AS `Symbol`,
  'Asclepias connivens' AS `ScientificName`,
  'largeflower milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun - Aug' AS `FloweringMonths`,
  '3' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias connivens'
);

-- ASCU6  Asclepias cutleri  (Cutler's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'd76daafb-b700-5a41-9b46-93779ae3807b' AS `Id`,
  'ASCU6' AS `Symbol`,
  'Asclepias cutleri' AS `ScientificName`,
  'Cutler''s milkweed' AS `CommonName`,
  'endemic to the Colorado Plateau' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr–Jun' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias cutleri'
);

-- ASEM2  Asclepias emoryi  (Emory's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'fbe7cd37-ce1f-555d-b2b4-08867a5e3c40' AS `Id`,
  'ASEM2' AS `Symbol`,
  'Asclepias emoryi' AS `ScientificName`,
  'Emory''s milkweed' AS `CommonName`,
  'similar in appearance to A. oenotheroides' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr–Jun' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias emoryi'
);

-- ASFE2  Asclepias feayi  (Florida milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '46905ea2-d5a3-5196-bb15-b569db8ccba4' AS `Id`,
  'ASFE2' AS `Symbol`,
  'Asclepias feayi' AS `ScientificName`,
  'Florida milkweed' AS `CommonName`,
  'Endemic to sandhills and scrubby flatwoods of the Florida Peninsula. Unlikely to become common at nurseries.' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Mar–Aug' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias feayi'
);

-- ASGL6  Asclepias glaucescens  (nodding milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'f34824d7-6b4b-5f32-83d8-d0cd29ecf17e' AS `Id`,
  'ASGL6' AS `Symbol`,
  'Asclepias glaucescens' AS `ScientificName`,
  'nodding milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun–Sep' AS `FloweringMonths`,
  '4' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias glaucescens'
);

-- ASHA5  Asclepias hallii  (Hall's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '02598f40-e8ca-5e4c-a996-4bc298a02912' AS `Id`,
  'ASHA5' AS `Symbol`,
  'Asclepias hallii' AS `ScientificName`,
  'Hall''s milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun–Aug' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias hallii'
);

-- ASIN5  Asclepias involucrata  (dwarf milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'bcb5f040-2d27-5e87-80fe-4ea3fe8edcfe' AS `Id`,
  'ASIN5' AS `Symbol`,
  'Asclepias involucrata' AS `ScientificName`,
  'dwarf milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr–Jul' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias involucrata'
);

-- ASLA11  Asclepias labriformis  (Utah milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '543b6caa-5d08-530c-9a65-caf972d1036b' AS `Id`,
  'ASLA11' AS `Symbol`,
  'Asclepias labriformis' AS `ScientificName`,
  'Utah milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Aug' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias labriformis'
);

-- ASLA13  Asclepias lanuginosa  (Sidecluster milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '7cf01786-3ffa-59f1-8449-06143093704f' AS `Id`,
  'ASLA13' AS `Symbol`,
  'Asclepias lanuginosa' AS `ScientificName`,
  'Sidecluster milkweed' AS `CommonName`,
  'This species isn''t abundant anywhere.' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Aug' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias lanuginosa'
);

-- ASLE10  Asclepias lemmonii  (Lemmon's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '2504d157-93cc-5b79-8dfb-333dd1b5f78c' AS `Id`,
  'ASLE10' AS `Symbol`,
  'Asclepias lemmonii' AS `ScientificName`,
  'Lemmon''s milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun–Sep' AS `FloweringMonths`,
  '5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias lemmonii'
);

-- ASMA13  Asclepias macrotis  (Longhood milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '7189f1bc-8395-5830-9adf-c6536083f1f4' AS `Id`,
  'ASMA13' AS `Symbol`,
  'Asclepias macrotis' AS `ScientificName`,
  'Longhood milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Oct' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias macrotis'
);

-- ASME6  Asclepias meadii  (Mead's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '4d8ad209-cd08-51ec-8372-fc0cfba6c7da' AS `Id`,
  'ASME6' AS `Symbol`,
  'Asclepias meadii' AS `ScientificName`,
  'Mead''s milkweed' AS `CommonName`,
  'Listed as a threatened species by the US Fish and Wildlife Service' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Jun' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias meadii'
);

-- ASNI4  Asclepias nivea  (Caribbean milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '20b7a8b0-ea89-545d-8291-8074aacc9a12' AS `Id`,
  'ASNI4' AS `Symbol`,
  'Asclepias nivea' AS `ScientificName`,
  'Caribbean milkweed' AS `CommonName`,
  'Native to Puerto Rico and US Virgin Islands' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Dec' AS `FloweringMonths`,
  '3' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias nivea'
);

-- ASNU2  Asclepias nummularia  (tufted milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'fb39062d-e5a2-5ca8-b0b7-aa1b97ba4d4f' AS `Id`,
  'ASNU2' AS `Symbol`,
  'Asclepias nummularia' AS `ScientificName`,
  'tufted milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Mar–Jun' AS `FloweringMonths`,
  '0.5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias nummularia'
);

-- ASNY2  Asclepias nyctaginifolia  (mojave milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'a1490a23-fb6b-5693-a3a9-ccbd58b7f074' AS `Id`,
  'ASNY2' AS `Symbol`,
  'Asclepias nyctaginifolia' AS `ScientificName`,
  'mojave milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Aug' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias nyctaginifolia'
);

-- ASPE14  Asclepias pedicellata  (savannah milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'ebe680ef-c040-58c8-82d5-86622d018e7f' AS `Id`,
  'ASPE14' AS `Symbol`,
  'Asclepias pedicellata' AS `ScientificName`,
  'savannah milkweed' AS `CommonName`,
  'One of the smallest milkweed species in North America, thus doesn''t have a major impact in your garden.' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May–Jun' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias pedicellata'
);

-- ASPR9  Asclepias prostrata  (prostrate milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '8ed3ec09-1f61-5fab-b59b-4a6450f16e6c' AS `Id`,
  'ASPR9' AS `Symbol`,
  'Asclepias prostrata' AS `ScientificName`,
  'prostrate milkweed' AS `CommonName`,
  'Listed as endangered by the US fish and Wildlife Service' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun - Aug' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias prostrata'
);

-- ASPU10  Asclepias pumila  (plains milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'f2f5d279-350e-59ff-9218-07a5bef4688f' AS `Id`,
  'ASPU10' AS `Symbol`,
  'Asclepias pumila' AS `ScientificName`,
  'plains milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun - Sep' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias pumila'
);

-- ASQU6  Asclepias quinquedentata  (slimpod milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '974e16ae-4887-5beb-8fa4-79f4a7bd7cbf' AS `Id`,
  'ASQU6' AS `Symbol`,
  'Asclepias quinquedentata' AS `ScientificName`,
  'slimpod milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun–Aug' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias quinquedentata'
);

-- ASRU2  Asclepias rusbyi  (Rusby's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '473f042f-a616-5ee1-954c-5debaa400885' AS `Id`,
  'ASRU2' AS `Symbol`,
  'Asclepias rusbyi' AS `ScientificName`,
  'Rusby''s milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun–Jul' AS `FloweringMonths`,
  '5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias rusbyi'
);

-- ASSC5  Asclepias scaposa  (Bear mountain milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'fba623d8-d845-528d-834c-904ec294cb69' AS `Id`,
  'ASSC5' AS `Symbol`,
  'Asclepias scaposa' AS `ScientificName`,
  'Bear mountain milkweed' AS `CommonName`,
  'Occurs in west Texas and northern Mexico' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Mar–Jun' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias scaposa'
);

-- ASSO3  Asclepias solanoana  (serpentine milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'e4a995ac-7734-5eec-b616-45b36ac2687b' AS `Id`,
  'ASSO3' AS `Symbol`,
  'Asclepias solanoana' AS `ScientificName`,
  'serpentine milkweed' AS `CommonName`,
  'Stems are prostrate (lay along the ground)' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias solanoana'
);

-- ASSP2  Asclepias sperryi  (Sperry's milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '2454bc46-cfbb-53e5-be57-80f9d08bbf94' AS `Id`,
  'ASSP2' AS `Symbol`,
  'Asclepias sperryi' AS `ScientificName`,
  'Sperry''s milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'April - June' AS `FloweringMonths`,
  '1' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias sperryi'
);

-- ASUN  Asclepias uncialis  (Wheel milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '93cbb025-8eb0-5928-9109-ca0f78387c8a' AS `Id`,
  'ASUN' AS `Symbol`,
  'Asclepias uncialis' AS `ScientificName`,
  'Wheel milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr–May' AS `FloweringMonths`,
  '0.5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias uncialis'
);

-- ASVI3  Asclepias viridula  (southern milkweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '2a3def09-6f1c-5ae8-a4f7-ceb394cef1c0' AS `Id`,
  'ASVI3' AS `Symbol`,
  'Asclepias viridula' AS `ScientificName`,
  'southern milkweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr–Jun' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Asclepias viridula'
);

-- BALU2  Bauhinia lunarioides  (Texasplume)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '2ff461e4-927b-56ac-90bb-7522d5aadd21' AS `Id`,
  'BALU2' AS `Symbol`,
  'Bauhinia lunarioides' AS `ScientificName`,
  'Texasplume' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Mar - May' AS `FloweringMonths`,
  '12' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Bauhinia lunarioides'
);

-- CABE5  Carphephorus bellidifolius  (sandywoods chaffhead)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'e09d5f5e-ee92-550b-b0c0-97114670e29f' AS `Id`,
  'CABE5' AS `Symbol`,
  'Carphephorus bellidifolius' AS `ScientificName`,
  'sandywoods chaffhead' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Aug - Oct' AS `FloweringMonths`,
  '2' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Carphephorus bellidifolius'
);

-- CATO6  Carphephorus tomentosus  (woolly chaffhead)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'baa66a4e-d5d3-56c3-a526-1da2bf001641' AS `Id`,
  'CATO6' AS `Symbol`,
  'Carphephorus tomentosus' AS `ScientificName`,
  'woolly chaffhead' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Aug - Oct' AS `FloweringMonths`,
  '2.5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Carphephorus tomentosus'
);

-- CICR2  Cirsium crassicaule  (Slough thistle)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'df6b1a98-e183-576c-916a-8cd989345673' AS `Id`,
  'CICR2' AS `Symbol`,
  'Cirsium crassicaule' AS `ScientificName`,
  'Slough thistle' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr - Jun' AS `FloweringMonths`,
  '10' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Cirsium crassicaule'
);

-- CIFO3  Cirsium fontinale var. campylon  (Mt. Hamilton thistle, Fountain Thistle)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'a1449a6f-8897-5610-a1d5-1e5c2c85376f' AS `Id`,
  'CIFO3' AS `Symbol`,
  'Cirsium fontinale var. campylon' AS `ScientificName`,
  'Mt. Hamilton thistle, Fountain Thistle' AS `CommonName`,
  'Jeps. var. fontinale is endangered' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr - Aug' AS `FloweringMonths`,
  '7' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Cirsium fontinale var. campylon'
);

-- EUMA3  Euphorbia marginata  (Snow on the mountain)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '0e5d75aa-0c31-531f-b432-06b7769b3f21' AS `Id`,
  'EUMA3' AS `Symbol`,
  'Euphorbia marginata' AS `ScientificName`,
  'Snow on the mountain' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jul - Oct' AS `FloweringMonths`,
  '5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Euphorbia marginata'
);

-- GOSU  Gonolobus suberosus  (anglepod)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '2e24eddf-513c-538b-8cf5-f93708641413' AS `Id`,
  'GOSU' AS `Symbol`,
  'Gonolobus suberosus' AS `ScientificName`,
  'anglepod' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Apr - Oct' AS `FloweringMonths`,
  '10' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Gonolobus suberosus'
);

-- HEAR5  Helenium arizonicum  (Arizona sneezeweed)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'a31cc9c8-789c-51bf-9600-c73d04cb20ac' AS `Id`,
  'HEAR5' AS `Symbol`,
  'Helenium arizonicum' AS `ScientificName`,
  'Arizona sneezeweed' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jul–Sep' AS `FloweringMonths`,
  '2.5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Helenium arizonicum'
);

-- HIMA11  Hibiscus martianus  (rosemallow)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'ddfc0ecb-2460-5958-936c-32eefca8b1ca' AS `Id`,
  'HIMA11' AS `Symbol`,
  'Hibiscus martianus' AS `ScientificName`,
  'rosemallow' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jan - Dec' AS `FloweringMonths`,
  '5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Hibiscus martianus'
);

-- MOAN3  Monardella antonina  (San Antonio Hills monardella)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'dfed898d-41af-5636-b61e-ebfd8e12ef3b' AS `Id`,
  'MOAN3' AS `Symbol`,
  'Monardella antonina' AS `ScientificName`,
  'San Antonio Hills monardella' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May - Aug' AS `FloweringMonths`,
  '1.67' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Monardella antonina'
);

-- MOBRL  Monardella breweri ssp. Lanceolata  (Brewer's monardella)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '7ba16a78-be70-5092-b156-96238d7eda1c' AS `Id`,
  'MOBRL' AS `Symbol`,
  'Monardella breweri ssp. Lanceolata' AS `ScientificName`,
  'Brewer''s monardella' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'May - Oct' AS `FloweringMonths`,
  '2.13' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Monardella breweri ssp. Lanceolata'
);

-- PYPY  Pycnanthemum pycnanthemoides  (Southern mountain mint)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '1bfbf42d-2357-5f0b-b1e9-6ee9b750933f' AS `Id`,
  'PYPY' AS `Symbol`,
  'Pycnanthemum pycnanthemoides' AS `ScientificName`,
  'Southern mountain mint' AS `CommonName`,
  'Easy to grow.' AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jun–Aug' AS `FloweringMonths`,
  '1.0-3.0' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Pycnanthemum pycnanthemoides'
);

-- SYEL  Symphyotrichum elliottii  (Marsh American-Aster)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  'ce726f41-8f2e-5b8a-af02-234c207c6316' AS `Id`,
  'SYEL' AS `Symbol`,
  'Symphyotrichum elliottii' AS `ScientificName`,
  'Marsh American-Aster' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Aug-Oct' AS `FloweringMonths`,
  '6' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Symphyotrichum elliottii'
);

-- SYGE2  Symphyotrichum georgianum  (Georgia aster)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '90bfdd28-7e58-5555-8ba7-4403d852dcc8' AS `Id`,
  'SYGE2' AS `Symbol`,
  'Symphyotrichum georgianum' AS `ScientificName`,
  'Georgia aster' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Oct-Nov' AS `FloweringMonths`,
  '1.5-3' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Symphyotrichum georgianum'
);

-- SYHE  Symphyotrichum hendersonii  (Lyall aster)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '426a29aa-c15c-5e47-a935-0623e609dbb1' AS `Id`,
  'SYHE' AS `Symbol`,
  'Symphyotrichum hendersonii' AS `ScientificName`,
  'Lyall aster' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Jul-Aug' AS `FloweringMonths`,
  '5' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Symphyotrichum hendersonii'
);

-- SYSI2  Symphyotrichum simmondsii  (Simmonds' aster)
INSERT INTO `plant`
  (`Id`, `Symbol`, `ScientificName`, `CommonName`, `Blurb`,
   `RecommendationScore`, `Showy`, `SuperPlant`,
   `FloweringMonths`, `Height`,
   `ImageUrl`, `HasImage`, `HasPreview`,
   `Source`, `Attribution`)
SELECT * FROM (SELECT
  '11dd3777-7bf0-516f-a368-cc4509ce5764' AS `Id`,
  'SYSI2' AS `Symbol`,
  'Symphyotrichum simmondsii' AS `ScientificName`,
  'Simmonds'' aster' AS `CommonName`,
  NULL AS `Blurb`,
  1 AS `RecommendationScore`,
  0 AS `Showy`,
  0 AS `SuperPlant`,
  'Oct-Jan' AS `FloweringMonths`,
  '4' AS `Height`,
  NULL AS `ImageUrl`,
  0 AS `HasImage`,
  0 AS `HasPreview`,
  'NPRC May 2026' AS `Source`,
  NULL AS `Attribution`
) AS t
WHERE NOT EXISTS (
  SELECT 1 FROM `plant` WHERE `ScientificName` = 'Symphyotrichum simmondsii'
);
