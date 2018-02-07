<?php
/**
 * Created by PhpStorm.
 * User: Xonshiz
 * Date: 05-02-2018
 * Time: 08:33 AM
 */

$user_state = $_POST['user_state'];
$user_country = $_POST['user_country'];
$user_blood_type = $_POST['user_blood_type'];


$curl = curl_init();

curl_setopt_array($curl, array(
    CURLOPT_PORT => "9925",
    CURLOPT_URL => "http://localhost:9925",
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_ENCODING => "",
    CURLOPT_MAXREDIRS => 10,
    CURLOPT_TIMEOUT => 30,
    CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
    CURLOPT_CUSTOMREQUEST => "POST",
    CURLOPT_POSTFIELDS => "{\n  \"operation\":\"sql\",\n  \"sql\": \"SELECT * FROM dev.blood_donation WHERE donation_blood_type = '$user_blood_type' AND donation_country = '$user_country' AND donation_state = '$user_state'\"\n}",
    CURLOPT_HTTPHEADER => array(
        'Authorization: Basic '. base64_encode("AdminUserName:AdminPassword"),
        "Content-Type: application/json"
    ),
));

$response = curl_exec($curl);
$err = curl_error($curl);

curl_close($curl);

if ($err) {
    echo "cURL Error #:" . $err;
} else {
    echo $response;
}