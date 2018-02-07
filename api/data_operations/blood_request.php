<?php
/**
 * Created by PhpStorm.
 * User: Xonshiz
 * Date: 04-02-2018
 * Time: 10:02 PM
 */
$donation_id = substr(md5(microtime()),rand(0,26),6);
$donation_requester_id = $_POST['donation_requester_id'];
$donation_blood_type = $_POST['donation_blood_type'];
$donation_country = $_POST['donation_country'];
$donation_state = $_POST['donation_state'];
$donation_area_code = $_POST['donation_area_code'];
$donation_phone = $_POST['donation_phone'];
$request_date = date("Y/m/d");
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
    CURLOPT_POSTFIELDS => "{\n  \"operation\":\"sql\",\n  \"sql\": \"INSERT INTO dev.blood_donation (donation_id, donor_id, requester_id, donation_date, donation_country, donation_state, donation_area_code, donation_blood_type, donation_phone, donation_status, request_date) VALUES('$donation_id', 'None', '$donation_requester_id', 'None', '$donation_country', '$donation_state', '$donation_area_code', '$donation_blood_type', '$donation_phone', 'requesting', '$request_date')\"\n}",
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