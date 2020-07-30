﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Outlook;

namespace JobEnter
{
    public partial class PanelForm : Form
    {
        public PanelForm()
        {
            InitializeComponent();
        }

        #region Variables

        private String name;
        private String number;
        private String address;
        private String email;
        private String city;
        private String state;
        private String zip;
        private String price;
        private String jobType;
        String currentDIR = Environment.CurrentDirectory;


        private int count = 0;

        private Settings set1 = new Settings();
        #endregion


        private void PanelForm_Load(object sender, EventArgs e)
        {
            // Spam Sheet Access Token : zja8p39vwwxywjoch3nuwpy1de
            // Spam Name               : TestSheet1 
            // Advsur API Key          : b2j1a696p2uyqw8apn44s3vpiq
            // Advsur Landing Page ID  : 3637774839506820
            // Advsur Landing Page Name: Landing Page 2020
            set1.AccessToken = "b2j1a696p2uyqw8apn44s3vpiq";
            set1.SheetName = "Landing Page 2020";
            count = 0;
        }

        /*
         * Prompts the user to select a location on their computer create the folder at
         * 
         * Note: -After getting the save to location, calls "CreateFolder" 
         *        Method to create a folder at the specified location
         *       -If user cancels and does not select save to location, return null
         * Returns: Absolute path of the folder that was created at the 
         *          specified location selected by the user
         */
        public String getSaveDialog(String address)
        {
            string folderPath = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.Description = "Select folder to save to";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                //String folderName = "Proposal for Services at " + address;
                String folderName = clientInfo1.Address + " " + clientInfo1.City;
                Console.WriteLine("Folder: " + folderName);
                String finalFolderPath = createFolder(folderPath, folderName);
                return finalFolderPath;
            }
            else
            {
                Console.WriteLine("Unable to save to file");
                return null;
            }
        }

        /* S
         * Create a folder at a specified path
         * 
         * Input: -Path: Location on the users computer to save the folder to
         *        -FolderName: A String value used to name the folder at the saved location
         *        
         * Output: Returns the String path of the folder, including the foldername appended 
         *         to the end of it
         *         If the Folder already exists at the specified location, returns null
         */
        public String createFolder(String path, String folderName)
        {
            if (!Directory.Exists(path + "\\" + folderName))
            {
                //Create a folder 
                Create_Folder folder2 = new Create_Folder(path, folderName);
                folder2.createFolder();
                return (path + "\\" + folderName);
            }
            else
            {
                MessageBox.Show("Folder already exists", "Error 1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }



        #region Button Click Methods

        // Variable "Count" is used to decide which pages will be visible and which will not,
        // and because there are only 4 pages, the maximum is set at 4
        private int countMax = 4;

        private void btnClientInfo_Click(object sender, EventArgs e)
        {
            count = 0;
            showHide(count);
        }

        private void btnJobType_Click(object sender, EventArgs e)
        {
            count = 1;
            showHide(count);
        }

        private void btnSelectServices_Click(object sender, EventArgs e)
        {
            count = 2;
            showHide(count);
        }

        private void btnPricingPage_Click(object sender, EventArgs e)
        {
            count = 3;
            showHide(count);
        }

        /*
         * Sets pages to either visible or not visible depedning on the "num" input
         * 
         * Input:
         *      Num: Used to change the current visibility of certain pages.
         *          Eg. Num = 1, set client info page to be visible, all others to be not
         */
        private void showHide(int num)
        {
            this.jobType = jobType1.getSelectedButton();
            switch (num)
            {
                case 0:
                    clientInfo1.Visible = true;
                    selectServices1.Visible = false;
                    verifyPage.Visible = false;
                    jobType1.Visible = false;
                    stakingPage1.Visible = false;

                    btnNext.Text = "Next";
                    break;
                case 1:
                    jobType1.Visible = true;
                    clientInfo1.Visible = false;
                    verifyPage.Visible = false;
                    selectServices1.Visible = false;
                    stakingPage1.Visible = false;

                    btnNext.Text = "Next";
                    break;
                case 2:
                    if (jobType1.getSelectedButton() == "One Stake" ||
                        jobType1.getSelectedButton() == "Two Stake" ||
                        jobType1.getSelectedButton() == "All Stake")
                    {
                        stakingPage1.Visible = true;
                    }
                    else
                    {
                        selectServices1.Visible = true;
                    }
                    verifyPage.Visible  = false;
                    clientInfo1.Visible = false;
                    jobType1.Visible    = false;

                    // Set dropdown box City and Checkbox List items
                    selectServices1.setComboSelected(clientInfo1.City);
                    selectServices1.setJobType(jobType1.getSelectedButton());
                    selectServices1.setBoxesShown();

                    btnNext.Text = "Next";
                    break;
                case 3:
                    verifyPage.Visible = true;
                    selectServices1.Visible = false;
                    stakingPage1.Visible = false;
                    jobType1.Visible = false;
                    clientInfo1.Visible = false;

                    // Sets the staking price on the verify page to be the value 
                    // of the staking price on the staking page
                    if (stakingPage1.STKPRice != "" || stakingPage1.STKPRice != null)
                        verifyPage.setStakePrice(stakingPage1.STKPRice);

                    // Select all neccesary headers
                    selectServices1.selectHeaders();
                    verifyPage.clearBox();          // Clear old input

                    // Add all user information to box
                    verifyPage.addToBox(clientInfo1.Name,
                                        clientInfo1.Number,
                                        clientInfo1.Email,
                                        clientInfo1.Address,
                                        clientInfo1.CountyBox,
                                        clientInfo1.City,
                                        clientInfo1.SpecialInstructions);
                    
                    btnNext.Text = "Save";
                    break;
                case 4:
                    count = 3;

                    //-------Make sure all necessary information is filled in-------
                    if(verifyConditions() == false)
                    {
                        break;
                    }
                    //========================================================
                    
                    verifyPage.addToBox("Saving...");
                    
                    // Location Strings
                    String absoluteFolderPath = getSaveDialog(clientInfo1.Address);
                    String absoluteFilePath   = absoluteFolderPath + "\\Proposal For Services at " + clientInfo1.Address;
                    String pdfPath            = absoluteFilePath + ".pdf";
                    String templateName       = getTemplateName(jobType1.getSelectedButton());

                    Console.WriteLine("Folder Path: " + absoluteFolderPath);
                    Console.WriteLine("File Path: " + absoluteFilePath);
                    Console.WriteLine("Template Name: " + templateName);

                    if (templateName == null)
                    {
                        MessageBox.Show("Could not find template file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        verifyPage.addToBox("Error");
                        break;
                    }

                    Console.WriteLine("Job Type: " + jobType1.getSelectedButton());


                    //--------------Save to File-----------------------------
                    if (absoluteFolderPath == null)
                        break;
                    else
                    {
                        switch (jobType1.getSelectedButton())
                        {
                            case "ALTA":
                                if (!FindAndReplace_ALTA(templateName, absoluteFilePath))
                                    break;
                                else
                                    verifyPage.addToBox("File saved successfully");
                                break;
                            case "One Stake":
                                goto case "All Stake";
                            case "Two Stake":
                                goto case "All Stake";
                            case "All Stake":
                                if (!FindAndReplace_Staking(templateName, absoluteFilePath))
                                    break;
                                else
                                    verifyPage.addToBox("File saved successfully");
                                break;
                            default:
                                if (!saveToWord(templateName, absoluteFilePath))
                                    break;
                                else
                                    verifyPage.addToBox("File saved successfully");
                                break;
                        }
                    }

                    //=====================================================

                    
                    //--------------Add CTF Letter to folder--------------
                    addCTFLetter(absoluteFolderPath);
                    //====================================================


                    //---------------Open File for Editing------------- 
                    openFile(absoluteFilePath + ".docx");

                    FileInfo fi1 = new FileInfo(absoluteFilePath + ".docx");
                    while (checkFileStatus(fi1))
                    { }
                    //=================================================
                    

                    //--------------Convert Word document to PDF----------
                    ConvertToPDF converter = new ConvertToPDF(absoluteFilePath + ".docx", pdfPath, absoluteFolderPath);
                    converter.convertToPDF();
                    //====================================================


                    //-------------------Open Draft in Outlook--------------
                    string[] name = clientInfo1.Name.Split(' ');
                    SendEmail sendEmail = new SendEmail(clientInfo1.Email, "info@advsur.com", clientInfo1.Address, "Hi " + name[0] + "-\nAttached is the proposal you requested.  Please let me know if you have any questions or if you would like to be added to our schedule. \n\nThank you for the opportunity.\n", pdfPath, null);
                    sendEmail.openOutlookWindow();
                    //======================================================

                    //----------------Add row to Smartsheet-----------------
                    verifyPage.addToBox("Adding to Smartsheet...");
                    updateAPI();
                    verifyPage.addToBox("Successfully added to Smartsheet");
                    //======================================================
                      
                    break;
            }
        }

        /*
         * Opens a specified word file in a new file
         * 
         * Input:
         *      -File Location: Specifies the file to be opened
         */
        private void openFile(string fileLocation)
        {
            try
            {
                if (fileLocation != null)
                {
                    Word.Application ap = new Word.Application();
                    Word.Document document = ap.Documents.Open(fileLocation);
                    ap.Visible = true;
                    ap.Activate();
                    ap.WindowState = Word.WdWindowState.wdWindowStateMaximize;
                }
            }
            catch (System.Exception ex){
                MessageBox.Show("Error while opening file: " + ex.Message);
            }
        }

        /*
         * In order to create the new file/folder, there needs to be an email address 
         * and a City provided to ensure proper naming conventions, and this methods checks
         * to ensure that both of those boxes are filled in.
         * 
         * Output: Returns false if any of the specified boxes are empty, otherwise it returns
         *         true
         */
        public Boolean verifyConditions()
        {
            if (clientInfo1.Address == "")
            {
                count = 0;
                showHide(count);
                MessageBox.Show("Please Enter a Address", "Address String Empty", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (clientInfo1.City == "")
            {
                count = 0;
                showHide(count);
                MessageBox.Show("Please Enter a City", "City String Empty", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (jobType1.getSelectedButton() == null)
            {
                count = 1;
                showHide(count);
                MessageBox.Show("Please Select a Job Type", "Job Type Null", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        /*
         * When next is clicked, this count goes up by one so long as it doesnt exceed the
         * maximun count.
         * 
         * Note: "ShowHide" is called after the count is upped
         */
        private void btnNext_Click(object sender, EventArgs e) 
        {
            if (!(count + 1 > countMax))
                count++;
            else
                count = countMax;

            showHide(count);
        }

        /*
        * When previous is clicked, this count goes down by one so long as it doesnt exceed
        * the maximun count.
        * 
        * Note: "ShowHide" is called after the count is dropped
        */
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (!(count - 1 < 0))
                count--;
            else
                count = 0;
            showHide(count);
        }

        /* 
         * Output: Returns the path for the specified template depending 
         * on what job type is selected
         *  -If no job type is selected, returns null
         */
        private String getTemplateName(String selected)
        {
            switch (selected)
            {
                case "One Stake":
                    return Path.Combine(currentDIR, "Templates", "StakingTemplate.docx");
                case "Two Stake":
                    return Path.Combine(currentDIR, "Templates", "StakingTemplate.docx");
                case "All Stake":
                    return Path.Combine(currentDIR, "Templates", "StakingTemplate.docx");
                case "Proposed":
                    return Path.Combine(currentDIR, "Templates", "Proposal Template.docx");
                case "New Home":
                    return Path.Combine(currentDIR, "Templates", "New Home Template.docx");
                case "Addition":
                    return Path.Combine(currentDIR, "Templates", "Additions Template.docx");
                case "Full":
                    return Path.Combine(currentDIR, "Templates", "fullTemplate.docx");
                case "Lot Split":
                    return Path.Combine(currentDIR, "Templates", "Lot Split Template.docx");
                case "Plat":
                    return Path.Combine(currentDIR, "Templates", "Plat Template.docx");
                case "ALTA":
                    return Path.Combine(currentDIR, "Templates", "ALTA Template.docx");
                default:
                    return null;
            }
        }

        #endregion

        #region Update Pages

        // Write methods that will be called when each page becomes visible
        // Ex. Set selected services label and city
        // .. Set Put all info into verify conditions


        #endregion

        #region Load Events

        private void verifyConditions1_Load(object sender, EventArgs e)
        {

        }


        private void verifyConditions2_Load(object sender, EventArgs e)
        {

        }

        private void selectServices1_Load(object sender, EventArgs e)
        {

        }

        #endregion



        #region Interacting with word document

        private void button2_Click(object sender, EventArgs e)
        {

        }

        /*
         * Since an Alta template is so different from a normal template, this method is
         * responsible for replacing all neccessary information on the Alta template
         * 
         * Input:
         *      -Template Name: String value for the path of the template the method will 
         *                      create a copy of
         *      -File Path: String path of the location to save the file to
         * 
         * Output: Returns false if the file fails, true if it doesn't
         */
        private Boolean FindAndReplace_ALTA(String templateName, String filePath)
        {
            try
            {
                String cityAddress = clientInfo1.Address + ", " + clientInfo1.City;
                CreateWordDoc doc1 = new CreateWordDoc(Path.Combine(currentDIR, templateName), filePath);
                String[] numbers = new String[] { "1", "2", "3", "4", "5", "6A", "6B", "7A", "7B", "7C", "8",
                                              "9", "10A", "10B", "11", "12", "13", "14", "15", "16", "17",
                                              "18", "19", "20", "21" };
                doc1.CreateDocument();

                doc1.FindAndReplace("<address>", cityAddress);
                doc1.FindAndReplace("<name>", clientInfo1.Name);
                doc1.FindAndReplace("<daysEST>", verifyPage.Days);
                doc1.FindAndReplace("<sumTotal>", verifyPage.Price);

                foreach (String s in selectServices1.getSelectedServices())
                {
                    String outStr = s.Split(']').First();
                    Console.WriteLine(outStr);
                    doc1.FindAndReplace("<" + outStr + ">", "Included");
                }
                foreach (String s in numbers)
                {
                    doc1.FindAndReplace("<" + s + ">", "Not Included");
                }

                doc1.closeAndSave();

                return true;

            }catch(SystemException ex)
            {
                return false;
            }
        }

        /*
         * Because Staking doesnt have any selectable services, a seperate find and replace method is 
         * required to ensure that the extra information is replaced.
         * 
         * Inputs:
         *      -Template Name: String path of the template to be copied
         *      -File Path: String path of where to save the new file to
         *      
         * Output: If the method fails, return false. If it does not, return true
         */
        private Boolean FindAndReplace_Staking(String templateName, String filePath)
        {
            try
            {
                CreateWordDoc doc1 = new CreateWordDoc(Path.Combine(currentDIR, templateName), filePath);
                doc1.CreateDocument();

                FindAndReplace_ClientInfo(doc1);

                Console.WriteLine("Corner: " + stakingPage1.Corner);
                doc1.FindAndReplace("<corner>", stakingPage1.Corner);
                doc1.FindAndReplace("<stakePrice>", verifyPage.StakePrice);

                doc1.closeAndSave();

                return true;
            }
            catch (SystemException ex)
            {
                return false;
            }
        }

        /*
         * Responsible for creating a new word document and calling all find and replace methods on it,
         * and then closing and saving the word document
         * 
         * Inputs:
         *      -Template Name: String path of the template to create a copy of
         *      -File Path: String path to save the new file to
         * 
         * Output: Returns true if the method runs correctly, and false if it encounters any errors
         */
        private Boolean saveToWord(String templateName, String filePath)
        {
            try
            {
                CreateWordDoc doc1 = new CreateWordDoc(Path.Combine(currentDIR, templateName), filePath);
                doc1.CreateDocument();

                // Find and replace client info
                FindAndReplace_ClientInfo(doc1);
                // Find and replace selected services and titles
                FindAndReplace_ServicesAndTitles(doc1);

                doc1.closeAndSave();

                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Save to Word Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /*
         * Replaces all the information in a word document related to the clinet information,
         * including name, phone number, email address, etc..
         * 
         * Input: 
         *      -Doc 1: Word document that the find and replace will be called on
         * 
         */
        private void FindAndReplace_ClientInfo(CreateWordDoc doc1)
        {
            String cityAddress = clientInfo1.Address + ", " + clientInfo1.City;
            try
            {
                //Find and replace
                if (clientInfo1.Name      != null) { doc1.FindAndReplace("<name>",       clientInfo1.Name); }
                if (clientInfo1.Address   != null) { doc1.FindAndReplace("<address>",    cityAddress); }
                if (clientInfo1.Number    != null) { doc1.FindAndReplace("<phone>",      clientInfo1.Number); }
                if (clientInfo1.Email     != null) { doc1.FindAndReplace("<email>",      clientInfo1.Email); }
                if (verifyPage.Price      != null) { doc1.FindAndReplace("<price>",      verifyPage.Price); }
                if (stakingPage1.STKPRice != null) { doc1.FindAndReplace("<stakePrice>", verifyPage.StakePrice); }
                if (verifyPage.Days       != null) { doc1.FindAndReplace("<days>",       verifyPage.Days); }

                if (clientInfo1.SpecialInstructions == "")
                    doc1.FindAndReplace("<instructions>", "None");
                else
                    doc1.FindAndReplace("<instructions>", clientInfo1.SpecialInstructions);

            }catch(System.Exception ex)
            {
                Console.WriteLine("Error in Client Info. " + ex.Message);
            }
        }

        /*
         * Finds and replaces everything in the word document based on what services 
         * are selected
         * 
         * Input:
         *      -Doc 1: Word document that the find and replace will be called on
         * Note: Gets all titles and lists as selected, stored in "Titles and services"
         *       -Stored as [-Title, Services, -Title, Services, etc..]
         *       -Services: "■ Service..\n ■ Service..\n"
         *       When it encounters a title, it replaces the Header on the word document 
         *       with the title, and then gets the string of services at one position after
         *       and replaces that with the related body on the work document
         */
        private void FindAndReplace_ServicesAndTitles(CreateWordDoc doc1)
        {
            try
            {
                List<String> titles         = selectServices1.getSelectedStrings();
                List<String> selectedTitles = selectServices1.getSelectedTitles();
                List<String> nonTitles      = selectServices1.getNotTitles();

                List<String> titlesAndServices = selectServices1.getTitlesAndList();

                for(int i = 0; i <= titlesAndServices.Count -1; i++)
                {
                    Console.WriteLine(titlesAndServices[i]);
                    if (titlesAndServices[i].StartsWith("-"))
                    {
                        var result = titlesAndServices[i].Substring(titlesAndServices[i].LastIndexOf("-") + 1);
                        if (titlesAndServices[i + 1] == "")
                        {
                            replaceTitles_All(doc1, result, titlesAndServices[i + 1], "");
                        }
                        else
                        {
                            System.Windows.Forms.Clipboard.SetText(titlesAndServices[i + 1]);
                            replaceTitles_All(doc1, result, "^c^p", result + "^p");
                        }
                        
                    }
                }
            }catch(System.Exception ex)
            {
                Console.WriteLine("Error 4. Line 575 - " + ex.Message);
            }
        }

        /*
         * Replaces all titles and services in a word document. All different job types 
         * have different Headers and Bodys, and this method helps to ensure there aren't
         * any replacable strings left in the word document
         * 
         * Inputs:
         *      -Doc 1: 
         *      -Title In: The title selected that will decide what services are replaced
         *      -Services In: The services string passed to replace the "<servicesBody>"
         *                    from the word document
         *      -Replace With: The title that string passed to replace the "<ServicesHeader>"
         *                     in the word document
         * 
         */
        private void replaceTitles_All(CreateWordDoc doc1, String titleIn,
                                        String servicesIn, String replaceWith)
        {
            switch (jobType1.getSelectedButton())
            {
                case "Lot Split":
                    if(titleIn == "Lot Splits")
                    {
                        doc1.FindAndReplace("<LotSplitHeader>", replaceWith);
                        doc1.FindAndReplace("<LotSplitBody>", servicesIn);
                    }
                    goto default;
                case "Full":
                    if(titleIn == "Natural Features")
                    {
                        doc1.FindAndReplace("<naturalHeader>", replaceWith);
                        doc1.FindAndReplace("<naturalBody>", servicesIn);
                    }
                    goto default;
                case "Addition":
                    switch (titleIn)
                    {
                        case "Addition":
                            doc1.FindAndReplace("<additionHeader>", replaceWith);
                            doc1.FindAndReplace("<additionBody>", servicesIn);
                            break;
                        case "House Staking":
                            doc1.FindAndReplace("<StakingHeader>", replaceWith);
                            doc1.FindAndReplace("<StakingBody>", servicesIn);
                            break;
                        case "Foundation as built":
                            doc1.FindAndReplace("<FoundationHeader>", replaceWith);
                            doc1.FindAndReplace("<FoundationBody>", servicesIn);
                            break;
                        case "Final as built":
                            doc1.FindAndReplace("<FinalHeader>", replaceWith);
                            doc1.FindAndReplace("<FinalBody>", servicesIn);
                            break;
                    }
                    goto default;
                case "New Home":
                    switch (titleIn)
                    {
                        case "New Homes":
                            doc1.FindAndReplace("<NewHomeHeader>", replaceWith);
                            doc1.FindAndReplace("<NewHomeBody>", servicesIn);
                            break;
                        case "House Staking":
                            doc1.FindAndReplace("<StakingHeader>", replaceWith);
                            doc1.FindAndReplace("<StakingBody>", servicesIn);
                            break;
                        case "Foundation as built":
                            doc1.FindAndReplace("<FoundationHeader>", replaceWith);
                            doc1.FindAndReplace("<FoundationBody>", servicesIn);
                            break;
                        case "Final as built":
                            doc1.FindAndReplace("<FinalHeader>", replaceWith);
                            doc1.FindAndReplace("<FinalBody>", servicesIn);
                            break;
                    }
                    goto default;
                case "Plat":
                    switch (titleIn)
                    {
                        case "Proposed Items":
                            doc1.FindAndReplace("<proposedHeader>", replaceWith);
                            doc1.FindAndReplace("<proposedBody>", servicesIn);
                            break;
                        case "Lot Splits":
                            doc1.FindAndReplace("<lotSplitHeader>", replaceWith);
                            doc1.FindAndReplace("<lotSplitBody>", servicesIn);
                            break;
                        case "SCOPE OF SERVICES [FINAL/RECORD PLAT]":
                            doc1.FindAndReplace("<PlatHeader>", replaceWith);
                            doc1.FindAndReplace("<PlatBody>", servicesIn);
                            break;
                    }
                    goto default;

                default:
                    switch (titleIn)
                    {
                        case "General Property Items":
                            doc1.FindAndReplace("<GeneralHeader>", replaceWith);
                            doc1.FindAndReplace("<GeneralBody>", servicesIn);
                            break;
                        case "Building and Improvements":
                            doc1.FindAndReplace("<build/improveHeader>", replaceWith);
                            doc1.FindAndReplace("<build/improveBody>", servicesIn);
                            break;
                        case "Utilities":
                            doc1.FindAndReplace("<utilHeader>", replaceWith);
                            doc1.FindAndReplace("<utilBody>", servicesIn);
                            break;
                        case "Natural Features":
                            doc1.FindAndReplace("<naturalHeader>", replaceWith);
                            doc1.FindAndReplace("<naturalBody>", servicesIn);
                            break;
                    }
                    break;
            }
        }

        /*
         * Checks weather or not a specified file is open
         * 
         * Inputs:
         *      -File Name: The path of the file that will be checked if its open
         * 
         * Output: True if the file is in use, false if it is not
         * 
         */
        public bool checkFileStatus(FileInfo fileName)
        {
            FileStream streamInput = null;

            try
            {
                streamInput = fileName.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException) { return true; }
            finally
            {
                if (streamInput != null)
                    streamInput.Close();
            }

            return false;
        }

        /*
         * Method for determining which CTF letters to add to the main job folder
         * 
         * Inputs:
         *      -String path for the folder to which the CTF letter(s) will be added to
         * 
         * Note: Methods call FindAndReplace_CTF() method depending on which CTF letter is to be addedd
         * 
         * Variable:
         *  -TemplateFile__: String path of the file to copy for ___ CTF Letter
         *  -DestFile___: String path of where the ___ CTF Letter will be saved to
         */
        private void addCTFLetter(String folderPath)
        {

            String templateFileTom = System.IO.Path.Combine(currentDIR, "Templates", "TomCTFLetter.docx");
            String templateFileWayne = System.IO.Path.Combine(currentDIR, "Templates", "WayneCTFLetter.docx");

            String destFileTom = Path.Combine(folderPath, "TomCTFLetter.docx");
            String destFileWayne = Path.Combine(folderPath, "WayneCTFLetter.docx");

            String pdfTom = Path.Combine(folderPath, "TomCTFLetter.pdf");
            String pdfWayne = Path.Combine(folderPath, "WayneCTFLetter.pdf");

            ConvertToPDF converterTom = new ConvertToPDF(destFileTom, pdfTom, folderPath);
            ConvertToPDF converterWayne = new ConvertToPDF(destFileWayne, pdfWayne, folderPath);

            Console.WriteLine("CTF Tom:" + destFileTom);
            Console.WriteLine("CTF Wayne:" + destFileWayne);

            switch (verifyPage.getCTF())
            {
                case "Tom":
                    FindAndReplace_CTF(templateFileTom, destFileTom, "Tom");
                    converterTom.convertToPDF();
                    break;
                case "Wayne":
                    FindAndReplace_CTF(templateFileWayne, destFileWayne, "Wayne");
                    converterWayne.convertToPDF();
                    break;
                case "WayneTom":
                    FindAndReplace_CTF(templateFileTom, destFileTom, "Tom");
                    FindAndReplace_CTF(templateFileWayne, destFileWayne, "Wayne");

                    // Convert to PDF
                    converterTom.convertToPDF();
                    converterWayne.convertToPDF();
                    break;
            }
        }

        /*
         * Method used to find and replace all the neccessary information from the CTF Letter template
         * 
         * Inputs:
         *      -Template File: String path of the Template to be copied 
         *      -Destination File: String path to save the new File to
         *      -Surveyor: Decides which CTF template will be copied and which surveyors information 
         *                 will be applied to it
         */
        private void FindAndReplace_CTF(String templateFile, String destinationFile,String surveyor)
        {
            String cityAddress = clientInfo1.Address + ", " + clientInfo1.City;
            try
            {
                if (surveyor == "Tom")
                {
                    Console.WriteLine("File: " + destinationFile);
                    CreateWordDoc docTom = new CreateWordDoc(templateFile, destinationFile);
                    docTom.CreateDocument();

                    //Find and replace
                    if (clientInfo1.Address != null) { docTom.FindAndReplace("<address>", cityAddress); }
                    if (this.jobType        != null) { docTom.FindAndReplace("<jobType>", jobType); }

                    docTom.closeAndSave();
                }
                else if (surveyor == "Wayne")
                {
                    Console.WriteLine("File: " + destinationFile);
                    CreateWordDoc docWayne = new CreateWordDoc(templateFile, destinationFile);
                    docWayne.CreateDocument();

                    //Find and replace
                    if (clientInfo1.Address != null) { docWayne.FindAndReplace("<address>", cityAddress); }
                    if (this.jobType        != null) { docWayne.FindAndReplace("<jobType>", jobType); }

                    docWayne.closeAndSave();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "F&R CTF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion

        //======================================================================//
        #region PDF Methods




        #endregion

        //======================================================================//
        #region Email Methods




        #endregion

        //======================================================================//
        #region API Methods

        /*
         * Adds a new row to smartsheet using the smartsheet API
         */
        private void updateAPI()
        {
            try
            {
                APIRequests apiInstance = new APIRequests(set1.SheetName, set1.AccessToken,
                    verifyPage.getGIS(clientInfo1.CountyBox), clientInfo1.Name, clientInfo1.Email, 
                    clientInfo1.Address, clientInfo1.City, clientInfo1.CountyBox, verifyPage.Price, 
                    clientInfo1.Number, clientInfo1.SpecialInstructions, DateTime.Today, jobType1.getSelectedButton());

                if (apiInstance.addRow())
                    MessageBox.Show("Row successfully added to smartsheet", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Error in adding row to smartsheet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Unable to update Smartsheet: \n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

        }

        /*
         * Clears all text boxes in the application and focuses on the first page so the user can 
         * start creating a new job template
         */
        private void newJobEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Update Count
            count = 0;
            showHide(count);

            clientInfo1.clearAll();
            jobType1.setSelectedButton("");
            selectServices1.clearAll();
            stakingPage1.clearAll();
            verifyPage.clearAll();
            // Code to clear everything for a new job entry
        }

        private void changeAccessTokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set1.Show();
        }
    }
}
