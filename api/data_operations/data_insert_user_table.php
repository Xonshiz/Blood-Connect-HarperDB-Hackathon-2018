<?php
/**
 * Created by PhpStorm.
 * User: Xonshiz
 * Date: 04-02-2018
 * Time: 07:53 PM
 */

$user_id = substr(md5(microtime()),rand(0,26),6);
$user_name = $_POST['user_name'];
$user_age = $_POST['user_age'];
$user_area_code = $_POST['user_area_code'];
$user_country = $_POST['user_country'];
$user_state = $_POST['user_state'];
$user_blood_type = $_POST['user_blood_type'];
$user_email = $_POST['user_email'];
$user_password = md5($_POST['user_password']);
$user_phone = $_POST['user_phone'];

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
    CURLOPT_POSTFIELDS => "{\n  \"operation\":\"sql\",\n  \"sql\": \"INSERT INTO dev.user_table (user_id, name, age, area_code, country, state, blood_type, email, phone, user_password) VALUES('$user_id', '$user_name', '$user_age', '$user_area_code', '$user_country', '$user_state', '$user_blood_type', '$user_email', '$user_phone', '$user_password')\"\n}",
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