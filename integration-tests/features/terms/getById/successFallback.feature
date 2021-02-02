Feature: GetById with useFallback=true with expected success.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, ID, and useFallback = true,
        validate the query result.

        Given path 'Terms', dictionary, audience, language, id
        And param useFallback = true
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience             | language | id       | expected                                                      |
            | cancer.gov | Patient              | en       | 43980    | success-fallback-lower-case-cancergov-patient-en-43980.json   |
            | Cancer.gov | Patient              | en       | 43980    | success-fallback-title-case-cancergov-patient-en-43980.json   |
            | cancer.gov | HealthProfessional   | en       | 556486   | success-fallback-lower-case-cancergov-hp-en-556486.json       |
            | Cancer.gov | HealthProfessional   | en       | 556486   | success-fallback-title-case-cancergov-hp-en-556486.json       |
            | genetics   | Patient              | en       | 43969    | success-fallback-lower-case-genetics-hp-en-43969.json         |
            | Genetics   | Patient              | en       | 43969    | success-fallback-title-case-genetics-hp-en-43969.json         |
            | genetics   | HealthProfessional   | en       | 44805    | success-fallback-lower-case-genetics-hp-en-44805.json         |
            | Genetics   | HealthProfessional   | en       | 44805    | success-fallback-title-case-genetics-hp-en-44805.json         |
            | cancer.gov | Patient              | es       | 43980    | success-fallback-lower-case-cancergov-patient-es-43980.json   |
            | Cancer.gov | Patient              | es       | 43980    | success-fallback-title-case-cancergov-patient-es-43980.json   |
            | cancer.gov | HealthProfessional   | es       | 556486   | success-fallback-lower-case-cancergov-hp-es-556486.json       |
            | Cancer.gov | HealthProfessional   | es       | 556486   | success-fallback-title-case-cancergov-hp-es-556486.json       |
            | genetics   | Patient              | es       | 43969    | success-fallback-lower-case-genetics-hp-es-43969.json         |
            | Genetics   | Patient              | es       | 43969    | success-fallback-title-case-genetics-hp-es-43969.json         |
            | genetics   | HealthProfessional   | es       | 44805    | success-fallback-lower-case-genetics-hp-es-44805.json         |
            | Genetics   | HealthProfessional   | es       | 44805    | success-fallback-title-case-genetics-hp-es-44805.json         |
