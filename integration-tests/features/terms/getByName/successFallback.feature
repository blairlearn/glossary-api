Feature: GetByName with useFallback=true with expected success.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, prettyUrlName, and useFallback = true,
        validate the query result.

        Given path 'Terms', dictionary, audience, language, prettyUrlName
        And param useFallback = true
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience             | language | prettyUrlName          | expected                                                                    |
            | cancer.gov | Patient              | en       | postoperative          | success-fallback-lower-case-cancergov-patient-en-postoperative.json         |
            | Cancer.gov | Patient              | en       | postoperative          | success-fallback-title-case-cancergov-patient-en-postoperative.json         |
            | cancer.gov | HealthProfessional   | en       | deleterious-mutation   | success-fallback-lower-case-cancergov-hp-en-deleterious-mutation.json       |
            | Cancer.gov | HealthProfessional   | en       | deleterious-mutation   | success-fallback-title-case-cancergov-hp-en-deleterious-mutation.json       |
            | genetics   | Patient              | en       | arteriogram            | success-fallback-lower-case-genetics-hp-en-arteriogram.json                 |
            | Genetics   | Patient              | en       | arteriogram            | success-fallback-title-case-genetics-hp-en-arteriogram.json                 |
            | genetics   | HealthProfessional   | en       | polymorphism           | success-fallback-lower-case-genetics-hp-en-polymorphism.json                |
            | Genetics   | HealthProfessional   | en       | polymorphism           | success-fallback-title-case-genetics-hp-en-polymorphism.json                |
            | cancer.gov | Patient              | es       | posoperatorio          | success-fallback-lower-case-cancergov-patient-es-posoperatorio.json         |
            | Cancer.gov | Patient              | es       | posoperatorio          | success-fallback-title-case-cancergov-patient-es-posoperatorio.json         |
            | cancer.gov | HealthProfessional   | es       | mutacion-deleterea     | success-fallback-lower-case-cancergov-hp-es-mutacion-deleterea.json         |
            | Cancer.gov | HealthProfessional   | es       | mutacion-deleterea     | success-fallback-title-case-cancergov-hp-es-mutacion-deleterea.json         |
            | genetics   | Patient              | es       | arteriograma           | success-fallback-lower-case-genetics-hp-es-arteriograma.json                |
            | Genetics   | Patient              | es       | arteriograma           | success-fallback-title-case-genetics-hp-es-arteriograma.json                |
            | genetics   | HealthProfessional   | es       | polimorfismo           | success-fallback-lower-case-genetics-hp-es-polimorfismo.json                |
            | Genetics   | HealthProfessional   | es       | polimorfismo           | success-fallback-title-case-genetics-hp-es-polimorfismo.json                |
