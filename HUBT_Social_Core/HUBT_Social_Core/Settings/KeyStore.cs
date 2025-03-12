using HUBT_Social_Core.Settings.@enum;

namespace HUBT_Social_Core.Settings
{
    public static class KeyStore
    {

        // Các key cho thông điệp liên quan Thong baos

        public const string NotationSomeOneLoginYouAccountTitle = "NotationSomeOneLoginYouAccountTittle";
        public const string NotationSomeOneLoginYouAccountBody = "NotationSomeOneLoginYouAccountBody";






        // Các key cho thông điệp liên quan đến đăng ký và xác minh 
        public const string InvalidInformation = "InvalidInformation";
        public const string RegistrationFailed = "RegistrationFailed";
        public const string UnableToSendOTP = "UnableToSendOTP";
        public const string RegistrationSuccess = "RegistrationSuccess";
        public const string StepOneVerificationSuccess = "StepOneVerificationSuccess";
        public const string AccountLocked = "AccountLocked";
        public const string LoginNotAllowed = "LoginNotAllowed";
        public const string TwoFactorRequired = "TwoFactorRequired";
        public const string InvalidCredentials = "InvalidCredentials";
        public const string VerificationSuccess = "VerificationSuccess";
        public const string OTPVerificationFailed = "OTPVerificationFailed";
        public const string InvalidLanguage = "InvalidLanguage";
        public const string LanguageChanged = "LanguageChanged";
        public const string LanguageChangeFailed = "LanguageChangeFailed";
        public const string DefaultLoginError = "DefaultLoginError";
        public const string DataNotAllowNull = "DataNotAllowNull";
        public const string ConfirmPasswordError = "ConfirmPasswordError";



        // Các key cho thông điệp liên quan đến tin nhắn
        public const string InvalidMessageData = "InvalidMessageData";
        public const string MessageSentSuccessfully = "MessageSentSuccessfully";
        public const string FailedToSendMessage = "FailedToSendMessage";
        public const string ChatRoomIdRequired = "ChatRoomIdRequired";
        public const string NoMessagesFound = "NoMessagesFound";
        public const string MessageDeletedSuccessfully = "MessageDeletedSuccessfully";
        public const string FailedToDeleteMessage = "FailedToDeleteMessage";
        public const string ChatRoomIdAndKeywordRequired = "ChatRoomIdAndKeywordRequired";
        public const string MessagesFoundSuccessfully = "MessagesFoundSuccessfully";

        // Các key cho thông điệp liên quan đến file và hình ảnh
        public const string ImageUploadedSuccessfully = "ImageUploadedSuccessfully";
        public const string FailedToUploadImage = "FailedToUploadImage";
        public const string FileUploadedSuccessfully = "FileUploadedSuccessfully";
        public const string FailedToUploadFile = "FailedToUploadFile";

        // Các key cho thông điệp liên quan đến nhóm trò chuyện
        public const string GroupNameRequired = "GroupNameRequired";
        public const string GroupCreatedSuccessfully = "GroupCreatedSuccessfully";
        public const string FailedToCreateGroup = "FailedToCreateGroup";
        public const string GroupNameUpdatedSuccessfully = "GroupNameUpdatedSuccessfully";
        public const string FailedToUpdateGroup = "FailedToUpdateGroup";
        public const string GroupDeletedSuccessfully = "GroupDeletedSuccessfully";
        public const string FailedToDeleteGroup = "FailedToDeleteGroup";
        public const string GroupNotFound = "GroupNotFound";

        // Các key cho thông điệp liên quan đến người dùng
        public const string UserAlreadyExists = "UserAlreadyExists";
        public const string UnableToStoreInDatabase = "UnableToStoreInDatabase";
        public const string EmailVerificationCodeSubject = "EmailVerificationCodeSubject";
        public const string EmailCannotBeEmpty = "EmailCannotBeEmpty";
        public const string EmailUpdated = "EmailUpdated";
        public const string EmailUpdateError = "EmailUpdateError";
        public const string PasswordCannotBeEmpty = "PasswordCannotBeEmpty";
        public const string PasswordUpdated = "PasswordUpdated";
        public const string PasswordUpdateError = "PasswordUpdateError";
        public const string NameUpdated = "NameUpdated";
        public const string NameUpdateError = "NameUpdateError";
        public const string PhoneNumberUpdated = "PhoneNumberUpdated";
        public const string PhoneNumberUpdateError = "PhoneNumberUpdateError";
        public const string GenderUpdated = "GenderUpdated";
        public const string GenderUpdateError = "GenderUpdateError";
        public const string DateOfBirthUpdated = "DateOfBirthUpdated";
        public const string DateOfBirthUpdateError = "DateOfBirthUpdateError";
        public const string GeneralUpdateSuccess = "GeneralUpdateSuccess";
        public const string GeneralUpdateError = "GeneralUpdateError";
        public const string CurrentPasswordAndUsernameCannotBeEmpty = "CurrentPasswordAndUsernameCannotBeEmpty";
        public const string PasswordCorrect = "PasswordCorrect";
        public const string PasswordIncorrect = "PasswordIncorrect";
        public const string UsernameCannotBeEmpty = "UsernameCannotBeEmpty";
        public const string UnAuthorize = "UnAuthorize";

        public const string AvatarUpdated = "AvatarUpdated";

        public const string AvatarUpdateError = "AvatarUpdateError";


        // Các key cho thông điệp lỗi và OTP
        public const string InvalidRequestError = "InvalidRequest";
        public const string OtpSent = "OtpSent";
        public const string OtpSendError = "OtpSendError";
        public const string OtpVerificationSuccess = "OtpVerificationSuccess";
        public const string OtpInvalid = "OtpInvalid";
        public const string UserNotFound = "UserNotFound";
        public const string UserInfoUpdatedSuccess = "UserInfoUpdatedSuccess";
        public const string UserInfoUpdateError = "UserInfoUpdateError";

        // Các key cho lỗi OTP và password
        public const string PasswordCheckEmptyError = "PasswordCheckEmptyError";
        public const string OtpVerifyEmptyError = "OtpVerifyEmptyError";

        public const string InvalidImageData = "InvalidImageData";
        public const string InvalidFileData = "InvalidFileData";
        //Token
        public const string TokenDeleted = "TokenDeleted";
        public const string TokenValid = "TokenValid";

        //ApiResponse
        public const string ApiNotFound = "ApiNotFound";
        public const string InternalServerError = "InternalServerError";
        public const string ApiForbibben = "ApiForbibben";
        public const string ApiError = "ApiError";

        public const string AvatarDefaultFemale1 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811655/qqgtqqr9igqnz6pj8zjn.jpg";
        public const string AvatarDefaultFemale2 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811807/ahd7jbrya441tv0wv4h2.jpg";
        public const string AvatarDefaultFemale3 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811831/khszyns5uzh6gf2zncsd.jpg";
        public const string AvatarDefaultFemale4 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811852/hhn3ljzoky2jcz4djivy.jpg";
        public const string AvatarDefaultFemale5 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811872/xthln1h8axw3nwrjqz0l.jpg";
        public const string AvatarDefaultFemale6 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811890/migfj5wqjmk4v6jeuada.jpg";
        public const string AvatarDefaultFemale7 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811913/bnlco4r2fwcsajwg7e7j.jpg";
        public const string AvatarDefaultFemale8 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811930/b6oi4predbmtbqnf2gi4.jpg";
        public const string AvatarDefaultMale1 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811971/ihaipmgkzjyfliavkq3e.jpg";
        public const string AvatarDefaultMale2 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811992/lbkn7ehp5aqdpkjkqzgf.jpg";
        public const string AvatarDefaultMale3 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731812010/fjt3fwuqk53lnb910qvu.jpg";
        public const string AvatarDefaultMale4 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731812037/txbbmvgtpyhnleij7g1s.jpg";
        public const string DefaultUserImage = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1732549977/jgldr2myysd7u6vx6sfy.jpg";

        // Các Key cho Email Template
        public const string EmailContentHeaderContent1 = "EmailContentHeaderContent1";
        public const string EmailContentHeaderContent2 = "EmailContentHeaderContent2";
        public const string EmailContentHeaderContent3 = "EmailContentHeaderContent3";
        public const string EmailContentHeaderContent4 = "EmailContentHeaderContent4";
        public const string EmailContentOTP0 = "EmailContentOTP0";
        public const string EmailContentOTP1 = "EmailContentOTP1";
        public const string EmailContentOTP2 = "EmailContentOTP2";
        public const string EmailContentOTP3 = "EmailContentOTP3";
        public const string EmailContentOTP4 = "EmailContentOTP4";
        public const string EmailContentOTP5 = "EmailContentOTP5";
        public const string EmailContentOTP6 = "EmailContentOTP6";
        public const string EmailContentFooter1 = "EmailContentFooter1";
        public const string EmailContentFooter2 = "EmailContentFooter2";
        public const string EmailContentFooter3 = "EmailContentFooter3";
        public const string EmailContentFooter4 = "EmailContentFooter4";

        public const string Email2Text0 = "Email2Text0";
        public const string Email2Text1 = "Email2Text1";
        public const string Email2Text2 = "Email2Text2";
        public const string Email2Text3 = "Email2Text3";
        public const string Email2Text4 = "Email2Text4";
        public const string Email2Text5 = "Email2Text5";
        public const string Email2Text6 = "Email2Text6";
        public const string Email2Text7 = "Email2Text7";
        public const string Email2Text8 = "Email2Text8";
        public const string Email2Text9 = "Email2Text9";
        public const string Email2Text10 = "Email2Text10";
        public const string Email2Text11 = "Email2Text11";
        public const string Email2Text12 = "Email2Text12";
        public const string Email2Text13 = "Email2Text13";



        public const string NotificationSend = "NotificationSend";
        public const string InvalidFCMToken = "InvalidFCMToken";
        public const string NotificationSendError = "NotificationSendError";
        public static string GetRandomAvatarDefault(Gender gender)
        {
            var random = new Random();
            List<string> avatarUrls = gender switch
            {
                Gender.Male => [
                                        AvatarDefaultMale1,
                        AvatarDefaultMale2,
                        AvatarDefaultMale3,
                        AvatarDefaultMale4
                                    ],
                Gender.Female => [
                                        AvatarDefaultFemale1,
                        AvatarDefaultFemale2,
                        AvatarDefaultFemale3,
                        AvatarDefaultFemale4,
                        AvatarDefaultFemale5,
                        AvatarDefaultFemale6,
                        AvatarDefaultFemale7,
                        AvatarDefaultFemale8
                                    ],
                _ => [DefaultUserImage],
            };

            // Chọn ngẫu nhiên từ danh sách
            return avatarUrls[random.Next(avatarUrls.Count)];
        }
        //Cấm động, CHẶT CU
        public const string EmailTemplate = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Email Template</title>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 0;
                            text-align: left;
                        }
                        .container {
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #fff;
                            border-radius: 10px;
                            padding: 10px;
                            background-color: #1b944e;
                        }
                        header {
                            display: table;
                            width: 100%;
                            height: 200px;
                            background-color: #dff0d8;
                            border-radius: 10px;
                        }
                        .header-left, .header-right {
                            display: table-cell;
                            vertical-align: middle;
                            padding: 0 0px;
                        }
                        .header-left img {
                            height: 125px;
                            width: auto;
                            border-radius: 30px;
                            margin-left: 10px;
                        }
                        .header-right h1 {
                            font-family: Amasis MT Pro Black;
                            color: black;
                            font-size: 75px;
                            margin: 0;
                            text-align: center;
                        }
                        .content {
                            background-color: #ffffff;
                            padding: 20px;
                            border-radius: 10px;
                            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                            margin-top: 10px;
                            background-color: #f4f4f4;
                        }
                        .content h2 {
                            font-size: 20px;
                            color: #1b944e;
                            margin-top: 0;
                        }
                        .otp-container {
                            text-align: center;
                            margin: 20px 0;
                            flex-wrap: nowrap;
                            
                        }
                        .otp-box {
                            display: inline-block;
                            width: 37px;
                            height: 37px;
                            background-color: #dff0d8;
                            text-align: center;
                            line-height: 40px;
                            font-size: 20px;
                            font-weight: bold;
                            border-radius: 5px;
                            color: #1b944e;
                            border: 2px solid #1b944e;
                            margin-right: 2px;
                        }
                        .otp-box:last-child {
                            margin-right: 0;
                        }
                        .footer {
                            background-color: #f4f4f4;
                            padding: 20px;
                            text-align: center;
                            font-size: 12px;
                            color: #666;
                            margin-top: 30px;
                            border-radius: 10px;
                        }
                        .footer a {
                            color: #1b944e;
                            text-decoration: none;
                        }
                        .footer-top {
                            text-align: center;
                            padding-bottom: 20px;
                        }
                        .social-icon img {
                            width: 30px;
                            height: 30px;
                            margin: 0 3px;
                        }
                        .footer-divider {
                            border-bottom: 1px solid #ddd;
                            margin: 20px 0;
                        }
                        .footer-bottom p {
                            margin: 5px 0;
                        }
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <header class=""header"">
                            <section class=""header-left"">
                                <img src=""https://res.cloudinary.com/dnx8aew1t/image/upload/v1733812934/vtpdccqnag2djickf47c.png"" alt=""HUBT Social"">
                            </section>
                            <section class=""header-right"">
                                <h1>HUBT Social</h1>
                            </section>
                        </header>
                        <table role=""presentation"" width=""100%"" class=""content"">
                            <tr>
                                <td>
                                    <h2>{{content-top0}} {{RecipientName}}</h2>
                                    <p>{{content-top1}}<br><br>{{content-top2}}</p>
                                    <div class=""otp-container"">
                                        <div class=""otp-box"">{{value-0}}</div>
                                        <div class=""otp-box"">{{value-1}}</div>
                                        <div class=""otp-box"">{{value-2}}</div>
                                        <div class=""otp-box"">{{value-3}}</div>
                                        <div class=""otp-box"">{{value-4}}</div>
                                        <div class=""otp-box"">{{value-5}}</div>
                                    </div>
                                    <p>{{content-bottom2}}</p>
                                    <p>{{content-bottom3}}<br><b style=""color: #1b944e"">{{content-bottom4}}</b></p>
                                </td>
                            </tr>
                        </table>
                        <table role=""presentation"" width=""100%"" class=""footer"">
                            <tr>
                                <td class=""footer-top"">
                                    <a href=""www.facebook.com/groups/8705552386231535/"" target=""_blank"" class=""social-icon"">
                                        <img src=""https://cdn2.iconfinder.com/data/icons/social-media-2285/512/1_Facebook_colored_svg_copy-128.png"" alt=""Facebook"">
                                    </a>
                                    <a href=""https://twitter.com"" target=""_blank"" class=""social-icon"">
                                        <img src=""https://cdn4.iconfinder.com/data/icons/social-media-black-white-2/1227/X-64.png"" alt=""X"">
                                    </a>
                                    <a href=""https://www.reddit.com/r/HUBT_Social/"" target=""_blank"" class=""social-icon"">
                                        <img src=""https://cdn3.iconfinder.com/data/icons/2018-social-media-logotypes/1000/2018_social_media_popular_app_logo_reddit-64.png"" alt=""Reddit"">
                                    </a>
                                    <a href=""https://www.tiktok.com/@hubtsocial"" target=""_blank"" class=""social-icon"">
                                        <img src=""https://cdn4.iconfinder.com/data/icons/social-media-flat-7/64/Social-media_Tiktok-64.png"" alt=""TikTok"">
                                    </a>
                                    <a href=""https://github.com"" target=""_blank"" class=""social-icon"">
                                        <img src=""https://cdn2.iconfinder.com/data/icons/social-icons-33/128/Github-64.png"" alt=""GitHub"">
                                    </a>
                                </td>
                            </tr>
                            <tr>
                                <td class=""footer-divider""></td>
                            </tr>
                            <tr>
                                <td class=""footer-bottom"">
                                    <p>{{footer1}}</p>
                                    <p>{{footer2}} <a href=""#"" onclick=""unsubscribe()"">{{footer3}}</a> {{footer4}}</p>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <script>
                        function unsubscribe() {
                            alert('Unsubscribed successfully.');
                        }
                    </script>
                </body>
                </html>";
    }
}
