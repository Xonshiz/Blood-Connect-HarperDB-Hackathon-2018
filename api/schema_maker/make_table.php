<?php
/**
 * Created by PhpStorm.
 * User: Xonshiz
 * Date: 04-02-2018
 * Time: 07:36 PM
 */
$table_name = $_POST['table_name'];
$schema_name = $_POST['schema_name'];
$hash_id_attribute = $_POST['hash_id_attribute'];

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
    CURLOPT_POSTFIELDS => "{\n  \"operation\":\"create_table\",\n  \"schema\":\"$schema_name\",\n  \"table\":\"$table_name\",\n  \"hash_attribute\": \"$hash_id_attribute\"\n}",
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