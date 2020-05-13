Feature: Autosuggest with expected success.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, and search text,
        validate the query result.

        Given path 'Autosuggest', dictionary, audience, language, search
        And param matchType = match
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience            | language | search      | match     | expected                                             |
            # Begins
            | Cancer.gov | Patient             | en       | child       | begins    | success-begins-cgov-patient-en-child.json            |
            | Cancer.gov | Patient             | es       | tumor       | begins    | success-begins-cgov-patient-es-tumor.json            |
            | Genetics   | HealthProfessional  | en       | gene        | begins    | success-begins-genetics-healthprof-en-gene.json      |
            # Contains
            | Cancer.gov | Patient             | en       | car         | contains  | success-contains-cgov-patient-en-car.json            |
            | Cancer.gov | Patient             | es       | crónica     | contains  | success-contains-cgov-patient-es-crónica.json        |
            | Genetics   | HealthProfessional  | en       | variant     | contains  | success-contains-genetics-healthprof-en-variant.json |
            # Contains - with embedded spaces
            | Cancer.gov | Patient             | en       | node biopsy | contains  | success-contains-cgov-patient-en-node-biopsy.json    |
            # Contains - match after dash
            | Cancer.gov | Patient             | en       | cel         | contains  | success-contains-cgov-patient-en-cel.json            |
