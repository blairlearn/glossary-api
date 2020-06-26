Feature: Search with expected success.

    Background:
        * url apiHost

    Scenario Outline: Given the dictionary, audience, language, and search text,
        validate the search result.

        Given path 'Terms', 'search', dictionary, audience, language, search
        And param matchType = match
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | dictionary | audience            | language | search      | match     | expected                                             |
            # Path separator in term name
            | Cancer.gov | Patient             | en       | 2/3         | contains  | success-contains-cgov-patient-en-two-thirds.json     |
            | Cancer.gov | Patient             | es       | 2/3         | contains  | success-contains-cgov-patient-es-two-thirds.json     |
            # Exact matches
            | Cancer.gov | Patient             | en       | MeV linear accelerator  | begins    | success-begins-exact-match-mev-linear.json     |
            | Cancer.gov | Patient             | en       | MeV linear accelerator  | contains  | success-contains-exact-match-mev-linear.json   |
