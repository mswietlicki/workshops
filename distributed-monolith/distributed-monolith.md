                                            Distributed MONOLITH
================================================================================================================

                                    +----------+         +----------+
                                    | Service  | ------> | Service  |
                                    |    A     | <------ |    B     |
                                    +----------+ \    /  +----------+
                                       |    |     \  /        /  \
                                       |     \     \/        /    \
                                       |      \    /\       /      |
                                       |       \  /  \     /       |
                                       v        \/    \   /        v
                                      +----------+     \ /     +----------+
                                      | Service  | ---> X ---- | Service  |
                                      |    C     |     / \     |    D     |
                                      +----------+    /   \    +----------+
                                                     /     \
                                                    v       v
                                                  +----------+
                                                  | Service  |
                                                  |    E     |
                                                  +----------+
 


                                                                                          BY
                                                                                          MATEUSZ ŚWIETLICKI

----------------------------------------------------------------------------------------------------------------


  Paulina                                                                                 BANK
     O                                                                                 __________
    /|\                                                                               /          \
    / \                                                                              /_          _\
                                                                                    // \        / \\
  - Name                                                                           /___\ \    / /___\
  - Surname                                                                       [_____] \__/ [_____]
  - IdNr                                                                          |     |      |     |
  - Address                                                                       |     |      |     |
  - TelephoneNr                                                                   |     |______|     |
  - FavoriteColor                                                                 |                  |
  - CatsName                                                                      |__________________|
  - Age                                                                         
  - LastMeal                                                                      - AccountNr
  - IsHungry                                                                      - AccountBalance
  - SleptWell                                                                     - ClientIdNr
  - ...                                                                           


ACTIONS:
  - Deposit money into the account
  - Withdraw X amount of cash



----------------------------------------------------------------------------------------------------------------

ACTIONS:
  - Print a Report (monthly)

REPORT TEMPLATE:

  Client Bank Account Report

  Client Name: {Name} {Surname}
  Account Number: {AccountNr}

  Account Balance: {AccountBalance}

  Current Status:
  The account is in good standing with a healthy balance. No suspicious activities detected.




                            Paulina                                     ____  
                               O    <------ What is your Surname? ---- / __ \
                              /|\                                      |    |
                              / \   ------- It's Smith --------------> | [] |




----------------------------------------------------------------------------------------------------------------





                            Paulina                                     ____  
                               O    <------ What is your Surname? ---- / __ \
                              /|\                                      |    |
                              / \   ------- It's Smith --------------> | [] |







                      1,000,000 clients.
                      1 min per call.
                      1000000 min / 60 min = 16666,66 h * 5 euro/h =  **83.333,00 euro**








----------------------------------------------------------------------------------------------------------------

                                                    BANK
                                                 __________
                                                /          \
                                               /_          _\
                                              // \        / \\
                                             /___\ \    / /___\
                                            [_____] \__/ [_____]
                                            |     |      |     |
                                            |     |      |     |
                                            |     |______|     |
                                            |                  |
                                            |__________________|
                                        
                                          - AccountNr
                                          - AccountBalance
                                          - ClientIdNr *        (readonly)
                                          - ClientTelephoneNr * (readonly)
                                          - ClientName *        (readonly)
                                          - ClientSurname *     (readonly)
                                          - ClientAddress *     (readonly)






----------------------------------------------------------------------------------------------------------------


                                             ---------------------------------------
                                            | I CHANGED MY SURNAME TO "ANDERSON"!!! |
                                  Paulina  / ---------------------------------------
                                     O    /
                                    /|\     
                                    / \  



   ____   
  / __ \  - Paulina changed her "Surname". I need to update my records.
  |    |
  | [] |  - ClientSurname <-- new value



  Beata
    O     - Paulina changed her "Surname". I need to update my records.
   /|\
   / \    - PaulinaSurname <-- new value





----------------------------------------------------------------------------------------------------------------


                                                  NO CACHING:




                            Paulina                                           Beata
                               O    <------ How much money do you have? ----    O 
                              /|\                                              /|\
                              / \                                              / \

                            Paulina                                            ____ 
                               O    ------ How much money do I have? ------>  / __ \
                              /|\                                             |    |
                              / \   <------ You have {AccountBalance} ------  | [] |

                            Paulina                                           Beata
                               O    ---- I have {AccountBalance} euros ---->    O 
                              /|\                                              /|\
                              / \                                              / \






----------------------------------------------------------------------------------------------------------------


                                                USING LOCAL DATA:

                            Paulina                                           Beata
                               O    <------ Can I borrow 50 euro  ----------    O 
                              /|\                                              /|\
                              / \   -------------- Sure ------------------->   / \

                            Paulina                                           ____  
                               O    --------- Send 50 euro to Beata -------> / __ \
                              /|\                                            |    |
                              / \   <------------------ Sure --------------- | [] |

                            Paulina 
                               O    
                              /|\   - AccountBalance = AccountBalance - 50
                              / \   

                            Paulina                                           Beata
                               O    <------ How much money do you have? ----    O 
                              /|\                                              /|\
                              / \   ---- I have {AccountBalance} euros ---->   / \




----------------------------------------------------------------------------------------------------------------

                                                GOOD PRACTICES
                                                ==============


                                  - Specify the OWNER of the data!

                                  - Make a readonly copy of everything you need
                                  - Inform others about changes to your data

                                  - Never share your database
                                  - Never allow anyone to change you data 
                                    without you knowledge

                                  - Make every call asynchronous
                                  - Call APIs only from Job or Workers
                                  - Never call APIs from your API




                                  YOUR MICROSERVICE IS INDEPENDENT ENTITY
                            DO NOT REALLY ON OTHERS WHEN FULFILLING YOUR FUNCTION




----------------------------------------------------------------------------------------------------------------
                                  +-----------+   +-----------+   +-----------+
                                  |           |   |           |   |           |
                                  |    Web    |   |   Phone   |   |  ExtApp   |
                                  |           |   |           |   |           |
                                  +-----+-----+   +-----+-----+   +-----+-----+
                                        |               |               |
                                        v               v               v
                                  +-------------------------------------------+
                                  |                 API Gateway               |
                                  +-------------------------------------------+
                                    |         |         |         |         |
                                    v         v         v         v         v
                                +-------+ +-------+ +-------+ +-------+ +-------+
                                |       | |       | |       | |       | |       |
                                |  App  | |  App  | |  App  | |  App  | |  App  |
                                |       | |       | |       | |       | |       |
                                +-------+ +-------+ +-------+ +-------+ +-------+
                                |       | |       | |       | |       | |       |
                                |  DB   | |  DB   | |  DB   | |  DB   | |  DB   |
                                |       | |       | |       | |       | |       |
                                +-------+ +-------+ +-------+ +-------+ +-------+
                                    ^         ^         ^         ^         ^
                                    |         |         |         |         |
                                    v         v         v         v         v
                                  +-------------------------------------------+
                                  |                 Event Bus                 |
                                  +-------------------------------------------+
----------------------------------------------------------------------------------------------------------------












                                                      THANKS

                                                MATEUSZ ŚWIETLICKI












                                            