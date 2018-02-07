<?php
/**
 * Created by PhpStorm.
 * User: Xonshiz
 * Date: 04-02-2018
 * Time: 08:19 PM
 */
$donation_id = $_POST['donation_id'];
$donation_donor_id = $_POST['donation_donor_id'];
//$donation_date = $_POST['donation_date'];
$donation_date = date("Y/m/d");
//$donation_requester_id = $_POST['donation_requester_id'];
//$donation_blood_type = $_POST['donation_blood_type'];
//$donation_country = $_POST['donation_country'];
//$donation_state = $_POST['donation_state'];
//$donation_area_code = $_POST['donation_area_code'];
//$donation_phone = $_POST['donation_phone'];

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
//    CURLOPT_POSTFIELDS => "{\n  \"operation\":\"sql\",\n  \"sql\": \"INSERT INTO dev.blood_donation (donation_id, donor_id, requester_id, donation_date, donation_country, donation_state, donation_area_code, donation_blood_type, donation_phone, donation_status) VALUES('$donation_id', '$donation_donor_id', '$donation_requester_id', '$donation_date', '$donation_country', '$donation_state', '$donation_area_code', '$donation_blood_type', '$donation_phone', 'donated')\"\n}",
    CURLOPT_POSTFIELDS => "{\n  \"operation\":\"sql\",\n  \"sql\": \"UPDATE dev.blood_donation SET donor_id = '$donation_donor_id', donation_date = '$donation_date', donation_status = 'donated' WHERE donation_id = '$donation_id'\"\n}",
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