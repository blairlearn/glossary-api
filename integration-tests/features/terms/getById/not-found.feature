Feature: GetById returns a status 404 when a term ID is not found.

    Background:
        * url apiHost

    Scenario Outline: Without fallback logic, unknown ID, valid dictionary
        Test for dictionary: '<dictionary>', audience: '<audience>', language: '<language>', id: "<id>"

        Given path 'Terms', dictionary, audience, language, id
        And param useFallback = false
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | dictionary | audience           | language | id  | expected                                                                                   |
            | cancer.gov | Patient            | en       | 100 | No match for dictionary 'cancer.gov', audience 'Patient', language 'en' and id '100'. |
            | genetics   | Patient            | en       | 100 | No match for dictionary 'genetics', audience 'Patient', language 'en' and id '100'. |
            | notset     | Patient            | en       | 100 | No match for dictionary 'notset', audience 'Patient', language 'en' and id '100'. |
            | cancer.gov | HealthProfessional | en       | 100 | No match for dictionary 'cancer.gov', audience 'HealthProfessional', language 'en' and id '100'. |
            | genetics   | HealthProfessional | en       | 100 | No match for dictionary 'genetics', audience 'HealthProfessional', language 'en' and id '100'. |
            | notset     | HealthProfessional | en       | 100 | No match for dictionary 'notset', audience 'HealthProfessional', language 'en' and id '100'. |
            | cancer.gov | Patient            | es       | 100 | No match for dictionary 'cancer.gov', audience 'Patient', language 'es' and id '100'. |
            | genetics   | Patient            | es       | 100 | No match for dictionary 'genetics', audience 'Patient', language 'es' and id '100'. |
            | notset     | Patient            | es       | 100 | No match for dictionary 'notset', audience 'Patient', language 'es' and id '100'. |
            | cancer.gov | HealthProfessional | es       | 100 | No match for dictionary 'cancer.gov', audience 'HealthProfessional', language 'es' and id '100'. |
            | genetics   | HealthProfessional | es       | 100 | No match for dictionary 'genetics', audience 'HealthProfessional', language 'es' and id '100'. |
            | notset     | HealthProfessional | es       | 100 | No match for dictionary 'notset', audience 'HealthProfessional', language 'es' and id '100'. |


    Scenario Outline: Without fallback logic, valid ID, unknown dictionary
        Test for audience: '<audience>', language: '<language>', id: "<id>"

        Given path 'Terms', 'unknown', audience, language, id
        And param useFallback = false
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | audience           | language | id    | expected                                                                                        |
            | Patient            | en       | 43966 | No match for dictionary 'unknown', audience 'Patient', language 'en' and id '43966'.            |
            | HealthProfessional | en       | 43966 | No match for dictionary 'unknown', audience 'HealthProfessional', language 'en' and id '43966'. |
            | Patient            | es       | 43966 | No match for dictionary 'unknown', audience 'Patient', language 'es' and id '43966'.            |
            | HealthProfessional | es       | 43966 | No match for dictionary 'unknown', audience 'HealthProfessional', language 'es' and id '43966'. |


    Scenario Outline: Using fallback logic, unknown term ID and valid dictionary.
        Test for dictionary: '<dictionary>', audience: '<audience>', language: '<language>', id: "<id>"

        Given path 'Terms', dictionary, audience, language, id
        And param useFallback = true
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | dictionary | audience           | language | id  | expected                                                                                   |
            | cancer.gov | Patient            | en       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | genetics   | Patient            | en       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | notset     | Patient            | en       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | cancer.gov | HealthProfessional | en       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | genetics   | HealthProfessional | en       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | notset     | HealthProfessional | en       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | cancer.gov | Patient            | es       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | genetics   | Patient            | es       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | notset     | Patient            | es       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | cancer.gov | HealthProfessional | es       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | genetics   | HealthProfessional | es       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |
            | notset     | HealthProfessional | es       | 100 | Could not find fallback term with ID '100' for any combination of dictionary and audience. |


    Scenario Outline: Using fallback logic, known term ID and unknown dictionary.
        Test for audience: '<audience>', language: '<language>', id: "<id>"

        Given path 'Terms', 'unknown', audience, language, id
        And param useFallback = true
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | audience           | language | id    | expected                                                                                                 |
            | Patient            | en       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'Patient'.            |
            | HealthProfessional | en       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'HealthProfessional'. |
            | Patient            | es       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'Patient'.            |
            | HealthProfessional | es       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'HealthProfessional'. |

